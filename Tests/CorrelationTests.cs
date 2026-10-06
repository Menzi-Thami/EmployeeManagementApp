using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EmployeeApi.Observability;
using EmployeeManagementApp.Application.Common.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Parsing;
using Shouldly;
using Xunit;

namespace EmployeeManagementApp.UnitTests
{
    public class CorrelationTests(EmployeeApiFactory factory) : IClassFixture<EmployeeApiFactory>
    {
        private sealed class CollectingSink : ILogEventSink
        {
            public ConcurrentQueue<LogEvent> Events { get; } = new();
            public void Emit(LogEvent logEvent) => Events.Enqueue(logEvent);
        }

        private static bool IsRequestCompletion(LogEvent e) =>
            e.MessageTemplate.Text.StartsWith("HTTP {RequestMethod} {RequestPath}", StringComparison.Ordinal);

        private static HttpRequestMessage WithTraceParent(string path, ActivityTraceId traceId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, path);
            request.Headers.Add("traceparent", $"00-{traceId.ToHexString()}-{ActivitySpanId.CreateRandom().ToHexString()}-01");
            return request;
        }

        [Fact]
        public async Task EveryResponse_CarriesTheW3CTraceIdHeader()
        {
            var response = await factory.CreateClient().GetAsync("/");

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var traceId = response.Headers.GetValues(ObservabilityExtensions.TraceIdHeader).Single();
            traceId.Length.ShouldBe(32);
            traceId.ShouldNotBe(new string('0', 32));
        }

        [Fact]
        public async Task IncomingTraceParent_IsContinued_AndEveryLogLineForTheRequestCarriesItsTraceId()
        {
            var sink = new CollectingSink();
            using var app = factory.WithWebHostBuilder(b =>
                b.ConfigureTestServices(s => s.AddSingleton<ILogEventSink>(sink)));
            var client = app.CreateClient();
            var traceId = ActivityTraceId.CreateRandom();

            var response = await client.SendAsync(WithTraceParent("/", traceId));

            response.Headers.GetValues(ObservabilityExtensions.TraceIdHeader).Single().ShouldBe(traceId.ToHexString());
            // The request-completion event is written after the response is handed back; give it a moment.
            for (var i = 0; i < 50 && !sink.Events.Any(IsRequestCompletion); i++)
            {
                await Task.Delay(20);
            }
            var requestEvents = sink.Events.Where(e => e.TraceId is not null).ToList();
            requestEvents.ShouldContain(e => IsRequestCompletion(e));
            requestEvents.ShouldAllBe(e => e.TraceId == traceId && e.SpanId != null);
        }

        [Fact]
        public async Task ApiError_HeaderSurvivesTheExceptionHandler_AndMatchesTheProblemDetailsTraceId()
        {
            factory.JobTitleRepository.GetJobTitleByIdAsync(4040, Arg.Any<CancellationToken>())
                .ThrowsAsync(new NotFoundException("Job title 4040 was not found."));

            var response = await factory.CreateClient().GetAsync("/api/JobTitle/4040");

            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            var traceId = response.Headers.GetValues(ObservabilityExtensions.TraceIdHeader).Single();
            // ProblemDetails uses the full traceparent form: 00-<trace id>-<span id>-<flags>.
            (await response.Content.ReadAsStringAsync()).ShouldContain($"\"traceId\":\"00-{traceId}-");
        }

        [Fact]
        public void ConsoleOutputTemplate_PrintsTheTraceId()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            var template = config["Serilog:WriteTo:0:Args:outputTemplate"];
            template.ShouldNotBeNull();
            var traceId = ActivityTraceId.CreateRandom();
            var logEvent = new LogEvent(DateTimeOffset.UnixEpoch, LogEventLevel.Information, null,
                new MessageTemplateParser().Parse("hello"), [], traceId, ActivitySpanId.CreateRandom());
            using var output = new StringWriter();

            new MessageTemplateTextFormatter(template).Format(logEvent, output);

            output.ToString().ShouldContain(traceId.ToHexString());
        }
    }
}
