using EmployeeManagementApp.Application.DTOs;
using EmployeeManagementApp.Domain.Models;

namespace EmployeeManagementApp.Application.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
        Task<EmployeeDto> GetEmployeeByIdAsync(int id);
        Task AddEmployeeAsync(EmployeeDto employeeDto);
        Task UpdateEmployeeAsync(EmployeeDto employeeDto);
        Task DeleteEmployeeAsync(int id);
        Task<IEnumerable<JobTitleDto>> GetAllJobTitlesAsync();
    }
}
