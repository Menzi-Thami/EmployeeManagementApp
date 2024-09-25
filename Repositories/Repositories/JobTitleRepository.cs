using Dapper;
using EmployeeManagementApp.Domain.Models; 
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagementApp.Infrastructure.Repositories
{
    public class JobTitleRepository : IJobTitleRepository 
    {
        private readonly string _connectionString;
        private readonly ILogger<JobTitleRepository> _logger;

        public JobTitleRepository(string connectionString, ILogger<JobTitleRepository> logger)
        {
            _connectionString = connectionString;

            _logger = logger;
        }

        public async Task<IEnumerable<JobTitle>> GetAllJobTitlesAsync()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    return await db.QueryAsync<JobTitle>("SELECT * FROM JobTitle");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching job titles");
                throw;
            }
        }
    }
}
