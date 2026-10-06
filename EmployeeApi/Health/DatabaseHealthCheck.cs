using EmployeeManagementApp.Application.Common.Interfaces;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace EmployeeApi.Health
{
    /// <summary>
    /// Readiness: opens a connection through the app's own factory (same connection string,
    /// same driver) and runs a trivial query. A failure is not caught here on purpose: the
    /// health check service records the exception as Unhealthy, and the response body only
    /// carries the status word, never the driver message.
    /// </summary>
    public sealed class DatabaseHealthCheck(IDbConnectionFactory connectionFactory) : IHealthCheck
    {
        public const string Name = "database";

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            await using var connection = connectionFactory.CreateConnection();
            await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            await command.ExecuteScalarAsync(cancellationToken);

            return HealthCheckResult.Healthy();
        }
    }
}
