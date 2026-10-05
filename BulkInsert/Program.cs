using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
                    services.AddHttpClient<IBulkInsertService, BulkInsertService>(client =>
                    {
                        client.Timeout = BulkInsertService.DownloadTimeout;
                        client.DefaultRequestHeaders.UserAgent.ParseAdd(BulkInsertService.UserAgent);
                    });
                });
    }
}
