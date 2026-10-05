using Dapper;
using EmployeeManagementApp.Domain.Models;
using EmployeeManagementApp.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
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

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync(CancellationToken cancellationToken)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    // One round trip: Dapper multi-mapping splits each row at the job title's Id.
                    const string sql = @"
                        SELECT e.Id, e.Name, e.Surname, e.JobTitleId, e.DateOfBirth,
                               jt.Id, jt.JobTitle
                        FROM Employee e
                        LEFT JOIN JobTitle jt ON jt.Id = e.JobTitleId
                        ORDER BY e.Id";
                    return await db.QueryAsync<Employee, JobTitles, Employee>(
                        new CommandDefinition(sql, cancellationToken: cancellationToken),
                        (employee, jobTitle) =>
                        {
                            employee.JobTitle = jobTitle;
                            return employee;
                        },
                        splitOn: "Id");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all employees");
                throw;
            }
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    const string sql = "SELECT Id, Name, Surname, JobTitleId, DateOfBirth FROM Employee WHERE Id = @Id";
                    return await db.QueryFirstOrDefaultAsync<Employee>(
                        new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching employee with ID {EmployeeId}", id);
                throw;
            }
        }

        public async Task AddEmployeeAsync(Employee employee, CancellationToken cancellationToken)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    const string sql = "INSERT INTO Employee (Name, Surname, JobTitleId, DateOfBirth) VALUES (@Name, @Surname, @JobTitleId, @DateOfBirth)";
                    await db.ExecuteAsync(new CommandDefinition(sql, employee, cancellationToken: cancellationToken));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding employee");
                throw;
            }
        }

        public async Task UpdateEmployeeAsync(Employee employee, CancellationToken cancellationToken)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    const string sql = "UPDATE Employee SET Name = @Name, Surname = @Surname, JobTitleId = @JobTitleId, DateOfBirth = @DateOfBirth WHERE Id = @Id";
                    await db.ExecuteAsync(new CommandDefinition(sql, employee, cancellationToken: cancellationToken));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating employee with ID {EmployeeId}", employee.Id);
                throw;
            }
        }

        public async Task DeleteEmployeeAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    const string sql = "DELETE FROM Employee WHERE Id = @Id";
                    await db.ExecuteAsync(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting employee with ID {EmployeeId}", id);
                throw;
            }
        }
    }
}
