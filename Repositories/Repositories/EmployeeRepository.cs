using Dapper;
using EmployeeManagementApp.Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration; 
using Serilog;
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
                    return await db.QueryAsync<Employee>("SELECT * FROM Employee");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching employees");
                throw;
            }
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    return await db.QueryFirstOrDefaultAsync<Employee>("SELECT * FROM Employee WHERE Id = @Id", new { Id = id });
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
                    var sql = "INSERT INTO Employee (Name, Surname, JobTitleId, DateOfBirth) VALUES (@Name, @Surname, @JobTitleId, @DateOfBirth)";
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
                    var sql = "UPDATE Employee SET Name = @Name, Surname = @Surname, JobTitleId = @JobTitleId, DateOfBirth = @DateOfBirth WHERE Id = @Id";
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
                    var sql = "DELETE FROM Employee WHERE Id = @Id";
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
