﻿using EmployeeManagementApp.Domain.Models;
using System.Collections.Generic;
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

        public async Task<IEnumerable<JobTitles>> GetAllJobTitlesAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM JobTitle"; 
                var jobTitles = await connection.QueryAsync<JobTitles>(sql);
                return jobTitles;
            }
        }

        public async Task<JobTitles> GetJobTitleByIdAsync(int jobTitleId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM JobTitle WHERE Id = @Id"; 
                var jobTitle = await connection.QueryFirstOrDefaultAsync<JobTitles>(sql, new { Id = jobTitleId });
                return jobTitle;
            }
        }
    }
}
