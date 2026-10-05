using Dapper;
using EmployeeManagementApp.Application.Common.Interfaces;
using EmployeeManagementApp.Application.Common.Models;

namespace EmployeeManagementApp.Infrastructure.Repositories
{
    public class ProjectRepository(IDbConnectionFactory connectionFactory) : IProjectRepository
    {
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
            await using var connection = connectionFactory.CreateConnection();
            const string sql = ProjectSummarySql + ProjectSummaryGroupBy + @"
                ORDER BY p.Id";

            var projects = await connection.QueryAsync<ProjectSummary>(
                new CommandDefinition(sql, cancellationToken: cancellationToken));
            return projects.ToList();
        }

        public async Task<ProjectSummary?> GetProjectByIdAsync(int id, CancellationToken cancellationToken)
        {
            await using var connection = connectionFactory.CreateConnection();
            const string sql = ProjectSummarySql + @"
                WHERE p.Id = @Id" + ProjectSummaryGroupBy;

            return await connection.QuerySingleOrDefaultAsync<ProjectSummary>(
                new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        }

        public async Task UpdateProjectCostAsync(int projectId, decimal cost, CancellationToken cancellationToken)
        {
            await using var connection = connectionFactory.CreateConnection();
            const string sql = "UPDATE Project SET Cost = @Cost WHERE Id = @ProjectId";
            await connection.ExecuteAsync(
                new CommandDefinition(sql, new { Cost = cost, ProjectId = projectId }, cancellationToken: cancellationToken));
        }
    }
}
