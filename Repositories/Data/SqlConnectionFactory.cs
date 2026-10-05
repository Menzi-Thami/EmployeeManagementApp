using System.Data.Common;
using EmployeeManagementApp.Application.Common.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EmployeeManagementApp.Infrastructure.Data
{
    public sealed class SqlConnectionFactory(IOptions<DatabaseOptions> options) : IDbConnectionFactory
    {
        private readonly string _connectionString = options.Value.DefaultConnection;

        public DbConnection CreateConnection() => new SqlConnection(_connectionString);
    }

    public static class DataServiceCollectionExtensions
    {
        /// <summary>
        /// Binds ConnectionStrings:DefaultConnection with ValidateOnStart, so a missing value
        /// fails at boot instead of on the first query, and registers the connection factory.
        /// </summary>
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            services.AddOptions<DatabaseOptions>()
                .BindConfiguration(DatabaseOptions.Section)
                .Validate(o => !string.IsNullOrWhiteSpace(o.DefaultConnection),
                    "ConnectionStrings:DefaultConnection is required. Set it in appsettings.json, user-secrets " +
                    "or the ConnectionStrings__DefaultConnection environment variable.")
                .ValidateOnStart();

            services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
            return services;
        }
    }
}
