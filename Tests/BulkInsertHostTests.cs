using System.Net.Http;
using System.Threading.Tasks;
using EmployeeManagementConsoleApp.Services;
using Microsoft.Extensions.DependencyInjection;
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
    }
}
