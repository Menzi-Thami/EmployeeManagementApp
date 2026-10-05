using EmployeeManagementApp.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagementApp.Application.Common.Interfaces
{
    public interface IEmployeeRepository
    {
        /// <summary>All employees, ordered by id, each with its <see cref="Employee.JobTitle"/> loaded.</summary>
        Task<IEnumerable<Employee>> GetAllEmployeesAsync();
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task AddEmployeeAsync(Employee employee);
        Task UpdateEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(int id);
    }
}
