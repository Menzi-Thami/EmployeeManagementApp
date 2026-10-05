using Dapper;
using EmployeeManagementApp.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace EmployeeManagementApp.Infrastructure.Calculators
{
    public class ProjectCostCalculator : IProjectCostCalculator
    {
        private readonly string _connectionString;
        private readonly ILogger<ProjectCostCalculator> _logger;

        public ProjectCostCalculator(string connectionString, ILogger<ProjectCostCalculator> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task<decimal> CalculateProjectCostAsync(int projectId, CancellationToken cancellationToken)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
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

                    return await db.ExecuteScalarAsync<decimal>(
                        new CommandDefinition(sql, new { ProjectId = projectId }, cancellationToken: cancellationToken));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calculating cost for project with ID {ProjectId}", projectId);
                throw;
            }
        }
    }
}
