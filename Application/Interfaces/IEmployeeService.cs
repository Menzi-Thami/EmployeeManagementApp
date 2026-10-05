using EmployeeManagementApp.Application.DTOs;

namespace EmployeeManagementApp.Application.Services
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(CancellationToken cancellationToken);
        Task<EmployeeDto> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken);
        Task AddEmployeeAsync(EmployeeDto employeeDto, CancellationToken cancellationToken);
        Task UpdateEmployeeAsync(EmployeeDto employeeDto, CancellationToken cancellationToken);
        Task DeleteEmployeeAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<JobTitleDto>> GetAllJobTitlesAsync(CancellationToken cancellationToken);
    }
}
