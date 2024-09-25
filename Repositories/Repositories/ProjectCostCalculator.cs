using Dapper;
using EmployeeManagementApp.Infrastructure.Interfaces;
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

        public decimal CalculateProjectCost(int projectId)
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

                    return db.ExecuteScalar<decimal>(sql, new { ProjectId = projectId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while calculating cost for project with ID {projectId}");
                throw;
            }
        }
    }
}
