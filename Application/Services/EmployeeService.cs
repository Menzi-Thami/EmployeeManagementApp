using AutoMapper;
using EmployeeManagementApp.Application.DTOs;
using EmployeeManagementApp.Domain.Models;
using EmployeeManagementApp.Infrastructure.Interfaces;
using EmployeeManagementApp.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagementApp.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;
        private readonly IJobTitleRepository _jobTitleRepository;
        private readonly IMapper _mapper; 

        public EmployeeService(IEmployeeRepository employeeRepository,
                               IJobTitleRepository jobTitleRepository,
                               ILogger<EmployeeService> logger,
                               IMapper mapper) 
        {
            _employeeRepository = employeeRepository;
            _jobTitleRepository = jobTitleRepository; 
            _logger = logger;
            _mapper = mapper; 
        }

        public async Task<IEnumerable<JobTitleDto>> GetAllJobTitlesAsync()
        {
            try
            {
                var jobTitles = await _jobTitleRepository.GetAllJobTitlesAsync();
                _logger.LogInformation("Fetched all job titles successfully.");
                var jobTitleDtos = _mapper.Map<IEnumerable<JobTitleDto>>(jobTitles);
                return jobTitleDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching job titles.");
                throw;
            }
        }

        // Add a new employee
        public async Task AddEmployeeAsync(EmployeeDto employeeDto)
        {
            try
            {
                var employee = _mapper.Map<Employee>(employeeDto);
                await _employeeRepository.AddEmployeeAsync(employee);  // Await the repository method

                _logger.LogInformation($"Employee {employee.Name} {employee.Surname} added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding employee.");
                throw;
            }
        }


        // Update an existing employee
        public async Task UpdateEmployeeAsync(EmployeeDto employeeDto)
        {
            try
            {
                var employee = _mapper.Map<Employee>(employeeDto);

                await _employeeRepository.UpdateEmployeeAsync(employee);
                _logger.LogInformation($"Employee {employee.Name} {employee.Surname} updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating employee with ID {employeeDto.Id}.");
                throw;
            }
        }

        // Delete an employee by ID
        public async Task DeleteEmployeeAsync(int employeeId)
        {
            try
            {
                await _employeeRepository.DeleteEmployeeAsync(employeeId);
                _logger.LogInformation($"Employee with ID {employeeId} deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting employee with ID {employeeId}.");
                throw;
            }
        }

        // Get all employees
        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            try
            {
                var employees = await _employeeRepository.GetAllEmployeesAsync();

                var employeeDtos = _mapper.Map<IEnumerable<EmployeeDto>>(employees);

                _logger.LogInformation("Fetched all employees successfully.");
                return employeeDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching employees.");
                throw;
            }
        }

        // Get employee by ID
        public async Task<EmployeeDto> GetEmployeeByIdAsync(int employeeId)
        {
            try
            {
                var employee = await _employeeRepository.GetEmployeeByIdAsync(employeeId);
                if (employee == null)
                {
                    _logger.LogWarning($"Employee with ID {employeeId} not found.");
                    return null;
                }

                var employeeDto = _mapper.Map<EmployeeDto>(employee);

                _logger.LogInformation($"Fetched employee with ID {employeeId} successfully.");
                return employeeDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching employee with ID {employeeId}.");
                throw;
            }
        }
    }
}
