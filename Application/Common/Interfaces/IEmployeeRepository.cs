using EmployeeManagementApp.Domain.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeManagementApp.Application.Common.Interfaces
{
    public interface IEmployeeRepository
    {
        /// <summary>All employees, ordered by id, each with its <see cref="Employee.JobTitle"/> loaded.</summary>
        Task<IEnumerable<Employee>> GetAllEmployeesAsync(CancellationToken cancellationToken);
        Task<Employee?> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken);
        Task AddEmployeeAsync(Employee employee, CancellationToken cancellationToken);
        Task UpdateEmployeeAsync(Employee employee, CancellationToken cancellationToken);
        Task DeleteEmployeeAsync(int id, CancellationToken cancellationToken);
    }
}
