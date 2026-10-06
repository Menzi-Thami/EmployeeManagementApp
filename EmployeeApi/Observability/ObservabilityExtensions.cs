using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace EmployeeApi.Observability
{
    public static class ObservabilityExtensions
    {
        public const string ServiceName = "EmployeeApi";
        public const string TraceIdHeader = "X-Trace-Id";

        /// <summary>
        /// OpenTelemetry traces and metrics for incoming requests, outgoing HttpClient calls, SQL
        /// commands and the .NET runtime. Logs stay on Serilog, which stamps every event with the
        /// current trace and span id. Nothing is exported unless OTEL_EXPORTER_OTLP_ENDPOINT is set
        /// (e.g. to an Aspire dashboard or a collector); the exporter reads the rest of its
        /// settings from the standard OTEL_* environment variables.
        /// </summary>
        public static IServiceCollection AddObservability(this IServiceCollection services, IConfiguration configuration)
        {
            var otel = services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(ServiceName))
                .WithTracing(tracing => tracing
                    // Probes would otherwise be most of the spans.
                    .AddAspNetCoreInstrumentation(o => o.Filter = ctx => !ctx.Request.Path.StartsWithSegments("/health"))
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation())
                .WithMetrics(metrics => metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation()
                    .AddRuntimeInstrumentation());

            if (!string.IsNullOrWhiteSpace(configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
            {
                otel.UseOtlpExporter();
            }

            return services;
        }

        /// <summary>
        /// Returns the request's W3C trace id as X-Trace-Id, so a user or caller can quote the one
        /// value that finds the request's logs and spans. Registered first and written in
        /// OnStarting because UseExceptionHandler clears headers set earlier in the pipeline.
        /// </summary>
        public static IApplicationBuilder UseTraceIdResponseHeader(this IApplicationBuilder app) =>
            app.Use(async (context, next) =>
            {
                var traceId = Activity.Current?.TraceId.ToHexString() ?? context.TraceIdentifier;
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers[TraceIdHeader] = traceId;
                    return Task.CompletedTask;
                });
                await next(context);
            });
    }
}
