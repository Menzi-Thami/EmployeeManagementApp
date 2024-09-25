using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using EmployeeManagementApp.Domain.Models;
using EmployeeManagementConsoleApp.Services;
using Microsoft.Extensions.Configuration;

namespace EmployeeManagementConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            var bulkInsertService = host.Services.GetRequiredService<IBulkInsertService>();

            await bulkInsertService.FetchAndBulkInsertProjectLocationsAsync(); 
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
     Host.CreateDefaultBuilder(args)
         .ConfigureServices((context, services) =>
         {
             services.AddLogging();
             services.AddScoped<IBulkInsertService>(provider =>
                 new BulkInsertService(provider.GetRequiredService<ILogger<BulkInsertService>>()));
         });

    }
}