using System.Data.Common;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using EmployeeApi.Health;
using EmployeeManagementApp.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.Extensions;
using Shouldly;
using Xunit;

namespace EmployeeManagementApp.UnitTests
{
    public class HealthCheckTests(EmployeeApiFactory factory) : IClassFixture<EmployeeApiFactory>
    {
        private static IDbConnectionFactory UnreachableDatabase()
        {
            var connection = Substitute.For<DbConnection>();
            connection.OpenAsync(Arg.Any<CancellationToken>())
                .ThrowsAsync(new InvalidOperationException("server not found"));
            var connectionFactory = Substitute.For<IDbConnectionFactory>();
            connectionFactory.CreateConnection().Returns(connection);
            return connectionFactory;
        }

        private static (IDbConnectionFactory Factory, DbCommand Command) ReachableDatabase()
        {
            var command = Substitute.For<DbCommand>();
            var connection = Substitute.For<DbConnection>();
            connection.Protected("CreateDbCommand").Returns(command);
            var connectionFactory = Substitute.For<IDbConnectionFactory>();
            connectionFactory.CreateConnection().Returns(connection);
            return (connectionFactory, command);
        }

        [Fact]
        public async Task Live_WithTheDatabaseDown_IsStillHealthy()
        {
            using var app = factory.WithWebHostBuilder(b =>
                b.ConfigureTestServices(s => s.AddSingleton(UnreachableDatabase())));

            var response = await app.CreateClient().GetAsync("/health/live");

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            (await response.Content.ReadAsStringAsync()).ShouldBe("Healthy");
        }

        [Fact]
        public async Task Ready_WithTheDatabaseDown_Returns503WithoutTheDriverMessage()
        {
            using var app = factory.WithWebHostBuilder(b =>
                b.ConfigureTestServices(s => s.AddSingleton(UnreachableDatabase())));

            var response = await app.CreateClient().GetAsync("/health/ready");

            response.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
            var body = await response.Content.ReadAsStringAsync();
            body.ShouldBe("Unhealthy");
            body.ShouldNotContain("server not found");
        }

        [Fact]
        public async Task Ready_WithTheDatabaseUp_Returns200()
        {
            var (connectionFactory, _) = ReachableDatabase();
            using var app = factory.WithWebHostBuilder(b =>
                b.ConfigureTestServices(s => s.AddSingleton(connectionFactory)));

            var response = await app.CreateClient().GetAsync("/health/ready");

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task DatabaseCheck_RunsACheapQueryThroughTheConnectionFactory()
        {
            var (connectionFactory, command) = ReachableDatabase();
            var sut = new DatabaseHealthCheck(connectionFactory);

            var result = await sut.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

            result.Status.ShouldBe(HealthStatus.Healthy);
            command.CommandText.ShouldBe("SELECT 1");
            await command.Received(1).ExecuteScalarAsync(Arg.Any<CancellationToken>());
        }
    }
}
