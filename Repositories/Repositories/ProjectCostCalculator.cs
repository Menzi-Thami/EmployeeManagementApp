using Dapper;
using EmployeeManagementApp.Application.Common.Interfaces;

namespace EmployeeManagementApp.Infrastructure.Calculators
{
    public class ProjectCostCalculator(IDbConnectionFactory connectionFactory) : IProjectCostCalculator
    {
        public async Task<decimal> CalculateProjectCostAsync(int projectId, CancellationToken cancellationToken)
        {
            await using var connection = connectionFactory.CreateConnection();
            const string sql = @"
                SELECT COALESCE(SUM(CASE
                    WHEN jt.JobTitle = 'Developer' THEN 2500
                    WHEN jt.JobTitle = 'DBA' THEN 3000
                    WHEN jt.JobTitle = 'Tester' THEN 1000
                    WHEN jt.JobTitle = 'Business Analyst' THEN 4500
                    ELSE 0
                END), 0) AS TotalCost
                FROM ProjectEmployee pe
                LEFT JOIN Employee e ON e.Id = pe.EmployeeID
                LEFT JOIN JobTitle jt ON jt.Id = e.JobTitleId
                WHERE pe.ProjectID = @ProjectId";

            return await connection.ExecuteScalarAsync<decimal>(
                new CommandDefinition(sql, new { ProjectId = projectId }, cancellationToken: cancellationToken));
        }
    }
}
