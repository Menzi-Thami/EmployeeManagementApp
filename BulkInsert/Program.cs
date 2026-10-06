using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using EmployeeManagementConsoleApp.Services;

namespace EmployeeManagementConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();

            // Starting the host wires Ctrl+C to ApplicationStopping, which cancels the work below.
            await host.StartAsync();
            var stopping = host.Services.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping;

            await using (var scope = host.Services.CreateAsyncScope())
            {
                var bulkInsertService = scope.ServiceProvider.GetRequiredService<IBulkInsertService>();
                await bulkInsertService.FetchAndBulkInsertProjectLocationsAsync(stopping);
            }

            await host.StopAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    var download = services.AddHttpClient<IBulkInsertService, BulkInsertService>(client =>
                        client.DefaultRequestHeaders.UserAgent.ParseAdd(BulkInsertService.UserAgent));

                    // Timeouts, retries with jittered backoff and a circuit breaker. The download is an
                    // idempotent GET, so retrying a transient failure is safe; unsafe methods are not retried.
                    download.AddStandardResilienceHandler(options =>
                    {
                        options.TotalRequestTimeout.Timeout = BulkInsertService.DownloadTimeout;
                        options.AttemptTimeout.Timeout = BulkInsertService.AttemptTimeout;
                        // Must be at least twice the attempt timeout, or options validation rejects it.
                        options.CircuitBreaker.SamplingDuration = BulkInsertService.AttemptTimeout * 2;
                        options.Retry.DisableForUnsafeHttpMethods();
                    });

                    // The handler sets HttpClient.Timeout to infinite. Put the same budget back: the
                    // handler only sees the call up to the response headers, and GetStringAsync reads
                    // the large body afterwards, which only HttpClient.Timeout bounds. Registered after
                    // the handler so it wins.
                    download.ConfigureHttpClient(client => client.Timeout = BulkInsertService.DownloadTimeout);
                });
    }
}
