using EmployeeManagementApp.Domain.Models;
using EmployeeManagementApp.Application.Common.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace EmployeeManagementApp.Infrastructure.Repositories
{
    public class JobTitleRepository(IDbConnectionFactory connectionFactory) : IJobTitleRepository
    {
        public async Task<IEnumerable<JobTitles>> GetAllJobTitlesAsync(CancellationToken cancellationToken)
        {
            await using var connection = connectionFactory.CreateConnection();
            const string sql = "SELECT Id, JobTitle FROM JobTitle ORDER BY Id";
            return await connection.QueryAsync<JobTitles>(
                new CommandDefinition(sql, cancellationToken: cancellationToken));
        }

        public async Task<JobTitles?> GetJobTitleByIdAsync(int jobTitleId, CancellationToken cancellationToken)
        {
            await using var connection = connectionFactory.CreateConnection();
            const string sql = "SELECT Id, JobTitle FROM JobTitle WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<JobTitles>(
                new CommandDefinition(sql, new { Id = jobTitleId }, cancellationToken: cancellationToken));
        }
    }
}
