using Dapper;
using EmployeeManagementApp.Application.Common.Interfaces;
using EmployeeManagementApp.Application.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration; 
using System.Data;
using System.Data.SqlClient;

namespace EmployeeManagementApp.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<ProjectRepository> _logger;

        public ProjectRepository(string connectionString, ILogger<ProjectRepository> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        // One row per project with the assigned employees' names aggregated. The CASE keeps a
        // project with no employees at NULL (CONCAT would turn the missing row into " ").
        private const string ProjectSummarySql = $@"
                SELECT p.Id, p.Name, p.Startdate AS StartDate, p.Enddate AS EndDate, p.Cost,
                       STRING_AGG(CASE WHEN e.Id IS NOT NULL THEN CONCAT(e.Name, ' ', e.Surname) END,
                                  '{ProjectSummary.EmployeeNameSeparator}')
                           WITHIN GROUP (ORDER BY e.Surname, e.Name) AS EmployeeNames
                FROM Project p
                LEFT JOIN ProjectEmployee pe ON p.Id = pe.ProjectID
                LEFT JOIN Employee e ON pe.EmployeeID = e.Id";

        private const string ProjectSummaryGroupBy = @"
                GROUP BY p.Id, p.Name, p.Startdate, p.Enddate, p.Cost";

        public async Task<IReadOnlyList<ProjectSummary>> GetAllProjectsAsync(CancellationToken cancellationToken)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    const string sql = ProjectSummarySql + ProjectSummaryGroupBy + @"
                ORDER BY p.Id";

                    var projects = await db.QueryAsync<ProjectSummary>(
                        new CommandDefinition(sql, cancellationToken: cancellationToken));
                    return projects.ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all projects");
                throw;
            }
        }



        public async Task<ProjectSummary?> GetProjectByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    const string sql = ProjectSummarySql + @"
                WHERE p.Id = @Id" + ProjectSummaryGroupBy;

                    return await db.QuerySingleOrDefaultAsync<ProjectSummary>(
                        new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching project with ID {ProjectId}", id);
                throw;
            }
        }

        public async Task UpdateProjectCostAsync(int projectId, decimal cost, CancellationToken cancellationToken)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    const string sql = "UPDATE Project SET Cost = @Cost WHERE Id = @ProjectId";
                    await db.ExecuteAsync(
                        new CommandDefinition(sql, new { Cost = cost, ProjectId = projectId }, cancellationToken: cancellationToken));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating cost for project with ID {ProjectId}", projectId);
                throw;
            }
        }
    }
}
