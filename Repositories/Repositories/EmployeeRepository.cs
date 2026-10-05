using Dapper;
using EmployeeManagementApp.Domain.Models;
using EmployeeManagementApp.Application.Common.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeManagementApp.Infrastructure.Repositories
{
    // Failures propagate unlogged: the web layer's exception handler logs each one once.
    public class EmployeeRepository(IDbConnectionFactory connectionFactory) : IEmployeeRepository
    {
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync(CancellationToken cancellationToken)
        {
            await using var connection = connectionFactory.CreateConnection();
            // One round trip: Dapper multi-mapping splits each row at the job title's Id.
            const string sql = @"
                SELECT e.Id, e.Name, e.Surname, e.JobTitleId, e.DateOfBirth,
                       jt.Id, jt.JobTitle
                FROM Employee e
                LEFT JOIN JobTitle jt ON jt.Id = e.JobTitleId
                ORDER BY e.Id";
            return await connection.QueryAsync<Employee, JobTitles, Employee>(
                new CommandDefinition(sql, cancellationToken: cancellationToken),
                (employee, jobTitle) =>
                {
                    employee.JobTitle = jobTitle;
                    return employee;
                },
                splitOn: "Id");
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken)
        {
            await using var connection = connectionFactory.CreateConnection();
            const string sql = "SELECT Id, Name, Surname, JobTitleId, DateOfBirth FROM Employee WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Employee>(
                new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        }

        public async Task AddEmployeeAsync(Employee employee, CancellationToken cancellationToken)
        {
            await using var connection = connectionFactory.CreateConnection();
            const string sql = "INSERT INTO Employee (Name, Surname, JobTitleId, DateOfBirth) VALUES (@Name, @Surname, @JobTitleId, @DateOfBirth)";
            await connection.ExecuteAsync(new CommandDefinition(sql, employee, cancellationToken: cancellationToken));
        }

        public async Task UpdateEmployeeAsync(Employee employee, CancellationToken cancellationToken)
        {
            await using var connection = connectionFactory.CreateConnection();
            const string sql = "UPDATE Employee SET Name = @Name, Surname = @Surname, JobTitleId = @JobTitleId, DateOfBirth = @DateOfBirth WHERE Id = @Id";
            await connection.ExecuteAsync(new CommandDefinition(sql, employee, cancellationToken: cancellationToken));
        }

        public async Task DeleteEmployeeAsync(int id, CancellationToken cancellationToken)
        {
            await using var connection = connectionFactory.CreateConnection();
            const string sql = "DELETE FROM Employee WHERE Id = @Id";
            await connection.ExecuteAsync(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        }
    }
}
