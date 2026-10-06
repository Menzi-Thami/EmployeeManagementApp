using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EmployeeManagementConsoleApp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;
using BulkInsertProgram = EmployeeManagementConsoleApp.Program;

namespace EmployeeManagementApp.UnitTests
{
    public class BulkInsertHostTests
    {
        // Development turns on ValidateScopes, which is what made the old root-provider resolution throw.
        private static readonly string[] DevelopmentArgs =
        [
            "--environment", "Development",
            "--ConnectionStrings:DefaultConnection", "Server=(localdb)\\unused;Database=unused"
        ];

        [Fact]
        public async Task CreateHostBuilder_InDevelopment_ResolvesTheServiceInsideAScope()
        {
            using var host = BulkInsertProgram.CreateHostBuilder(DevelopmentArgs).Build();

            await using var scope = host.Services.CreateAsyncScope();
            var service = scope.ServiceProvider.GetRequiredService<IBulkInsertService>();

            service.ShouldBeOfType<BulkInsertService>();
        }

        [Fact]
        public void CreateHostBuilder_ConfiguresTheTypedClientWithAnExplicitTimeout()
        {
            using var host = BulkInsertProgram.CreateHostBuilder(DevelopmentArgs).Build();
            var factory = host.Services.GetRequiredService<IHttpClientFactory>();

            using var client = factory.CreateClient(nameof(IBulkInsertService));

            client.Timeout.ShouldBe(BulkInsertService.DownloadTimeout);
            client.DefaultRequestHeaders.UserAgent.ToString().ShouldNotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreateHostBuilder_ResilienceTimeouts_AgreeWithTheClientTimeout()
        {
            using var host = BulkInsertProgram.CreateHostBuilder(DevelopmentArgs).Build();

            var options = host.Services.GetRequiredService<IOptionsMonitor<HttpStandardResilienceOptions>>()
                .Get(ResiliencePipelineName);

            options.TotalRequestTimeout.Timeout.ShouldBe(BulkInsertService.DownloadTimeout);
            options.AttemptTimeout.Timeout.ShouldBe(BulkInsertService.AttemptTimeout);
            options.AttemptTimeout.Timeout.ShouldBeLessThan(options.TotalRequestTimeout.Timeout);
            options.CircuitBreaker.SamplingDuration.ShouldBeGreaterThanOrEqualTo(options.AttemptTimeout.Timeout * 2);
        }

        [Fact]
        public async Task Download_AfterATransientFailure_IsRetried()
        {
            var feed = new StubFeed(HttpStatusCode.ServiceUnavailable, HttpStatusCode.OK);
            using var host = HostWithFeed(feed);
            using var client = host.Services.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(IBulkInsertService));

            var body = await client.GetStringAsync("https://feed.test/sources");

            body.ShouldBe("ok");
            feed.Calls.ShouldBe(2);
        }

        [Fact]
        public async Task UnsafeMethods_AreNotRetried()
        {
            var feed = new StubFeed(HttpStatusCode.ServiceUnavailable, HttpStatusCode.OK);
            using var host = HostWithFeed(feed);
            using var client = host.Services.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(IBulkInsertService));

            using var response = await client.PostAsync("https://feed.test/sources", new StringContent("{}"));

            response.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
            feed.Calls.ShouldBe(1);
        }

        private const string ResiliencePipelineName = nameof(IBulkInsertService) + "-standard";

        private static IHost HostWithFeed(StubFeed feed) =>
            BulkInsertProgram.CreateHostBuilder(DevelopmentArgs)
                .ConfigureServices(services =>
                {
                    services.AddHttpClient(nameof(IBulkInsertService)).ConfigurePrimaryHttpMessageHandler(() => feed);
                    services.Configure<HttpStandardResilienceOptions>(ResiliencePipelineName, o => o.Retry.Delay = TimeSpan.Zero);
                })
                .Build();

        private sealed class StubFeed(params HttpStatusCode[] responses) : HttpMessageHandler
        {
            private int _calls;
            public int Calls => _calls;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var call = Interlocked.Increment(ref _calls);
                var status = responses[Math.Min(call, responses.Length) - 1];
                return Task.FromResult(new HttpResponseMessage(status) { Content = new StringContent(status == HttpStatusCode.OK ? "ok" : "") });
            }
        }
    }
}
