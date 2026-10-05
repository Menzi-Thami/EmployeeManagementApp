using System.Collections.Generic;
using EmployeeManagementApp.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace EmployeeManagementApp.UnitTests
{
    public class DatabaseConfigurationTests(EmployeeApiFactory factory) : IClassFixture<EmployeeApiFactory>
    {
        [Fact]
        public void Startup_WithoutAConnectionString_FailsAtBoot()
        {
            using var misconfigured = factory.WithWebHostBuilder(builder =>
                builder.ConfigureAppConfiguration((_, config) =>
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["ConnectionStrings:DefaultConnection"] = ""
                    })));

            var ex = Should.Throw<OptionsValidationException>(() => misconfigured.CreateClient());

            ex.Message.ShouldContain("ConnectionStrings:DefaultConnection");
        }

        [Fact]
        public void ConnectionFactory_CreatesMicrosoftDataSqlClientConnections()
        {
            const string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=Any;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True";
            var sut = new SqlConnectionFactory(Options.Create(new DatabaseOptions { DefaultConnection = connectionString }));

            using var connection = sut.CreateConnection();

            connection.ShouldBeOfType<SqlConnection>();
            connection.ConnectionString.ShouldBe(connectionString);
        }

        [Fact]
        public void ShippedLocalDbConnectionString_StatesEncryptAndTrustsTheLocalDbCertificate()
        {
            // Microsoft.Data.SqlClient defaults to Encrypt=Mandatory; LocalDB's self-signed certificate
            // only passes because the dev string says TrustServerCertificate=True explicitly.
            var options = factory.Services.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            var builder = new SqlConnectionStringBuilder(options.DefaultConnection);

            builder.DataSource.ShouldStartWith("(localdb)");
            builder.Encrypt.ShouldBe(SqlConnectionEncryptOption.Mandatory);
            builder.TrustServerCertificate.ShouldBeTrue();
        }
    }
}
