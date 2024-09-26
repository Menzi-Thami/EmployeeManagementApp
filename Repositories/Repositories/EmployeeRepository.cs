using Dapper;
using EmployeeManagementApp.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace EmployeeManagementApp.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<EmployeeRepository> _logger;

        public EmployeeRepository(string connectionString, ILogger<EmployeeRepository> logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    const string sql = "SELECT * FROM Employee";
                    return await db.QueryAsync<Employee>(sql);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all employees");
                throw;
            }
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesWithJobTitlesAsync()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    const string sql = @"
                        SELECT e.*, jt.JobTitle 
                        FROM Employee e
                        LEFT JOIN JobTitle jt ON e.JobTitleId = jt.Id";
                    return await db.QueryAsync<Employee>(sql);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching employees with job titles");
                throw;
            }
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    const string sql = "SELECT * FROM Employee WHERE Id = @Id";
                    return await db.QueryFirstOrDefaultAsync<Employee>(sql, new { Id = id });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching employee with ID {id}");
                throw;
            }
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    const string sql = "INSERT INTO Employee (Name, Surname, JobTitleId, DateOfBirth) VALUES (@Name, @Surname, @JobTitleId, @DateOfBirth)";
                    await db.ExecuteAsync(sql, employee);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding employee");
                throw;
            }
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    const string sql = "UPDATE Employee SET Name = @Name, Surname = @Surname, JobTitleId = @JobTitleId, DateOfBirth = @DateOfBirth WHERE Id = @Id";
                    await db.ExecuteAsync(sql, employee);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating employee with ID {employee.Id}");
                throw;
            }
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    const string sql = "DELETE FROM Employee WHERE Id = @Id";
                    await db.ExecuteAsync(sql, new { Id = id });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting employee with ID {id}");
                throw;
            }
        }
    }
}
