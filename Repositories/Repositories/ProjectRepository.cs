using Dapper;
using EmployeeManagementApp.Domain.Models;
using EmployeeManagementApp.Infrastructure.Interfaces;
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

        public IEnumerable<Project> GetAllProjects()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    return db.Query<Project>("SELECT * FROM Project");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all projects");
                throw;
            }
        }

        public Project GetProjectById(int id)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    return db.QuerySingleOrDefault<Project>("SELECT * FROM Project WHERE Id = @Id", new { Id = id });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching project with ID {id}");
                throw;
            }
        }

        public void UpdateProjectCost(int projectId, decimal cost)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    const string sql = "UPDATE Project SET Cost = @Cost WHERE Id = @ProjectId";
                    db.Execute(sql, new { Cost = cost, ProjectId = projectId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating cost for project with ID {projectId}");
                throw;
            }
        }
    }
}
