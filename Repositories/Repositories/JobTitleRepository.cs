using EmployeeManagementApp.Domain.Models;
using EmployeeManagementApp.Application.Common.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using System.Data;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;

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

        public async Task<IEnumerable<JobTitles>> GetAllJobTitlesAsync(CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                const string sql = "SELECT Id, JobTitle FROM JobTitle ORDER BY Id";
                return await connection.QueryAsync<JobTitles>(
                    new CommandDefinition(sql, cancellationToken: cancellationToken));
            }
        }

        public async Task<JobTitles?> GetJobTitleByIdAsync(int jobTitleId, CancellationToken cancellationToken)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                const string sql = "SELECT Id, JobTitle FROM JobTitle WHERE Id = @Id";
                return await connection.QueryFirstOrDefaultAsync<JobTitles>(
                    new CommandDefinition(sql, new { Id = jobTitleId }, cancellationToken: cancellationToken));
            }
        }
    }
}
