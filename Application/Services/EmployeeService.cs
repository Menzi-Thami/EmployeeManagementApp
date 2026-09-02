using AutoMapper;
using EmployeeManagementApp.Application.Common.Exceptions;
using EmployeeManagementApp.Application.Common.Interfaces;
using EmployeeManagementApp.Application.DTOs;
using EmployeeManagementApp.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace EmployeeManagementApp.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IJobTitleRepository _jobTitleRepository;
        private readonly ILogger<EmployeeService> _logger;
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

        // Add a new employee with job title
        public async Task AddEmployeeAsync(EmployeeDto employeeDto)
        {
            var jobTitle = await _jobTitleRepository.GetJobTitleByIdAsync(employeeDto.JobTitleId);
            if (jobTitle != null)
            {
                employeeDto.JobTitleName = jobTitle.JobTitle;
            }
            else
            {
                _logger.LogWarning("Job title {JobTitleId} not found for new employee.", employeeDto.JobTitleId);
            }

            var employee = _mapper.Map<Employee>(employeeDto);
            await _employeeRepository.AddEmployeeAsync(employee);
            // Log identifiers only — not the employee's name/surname (PII).
            _logger.LogInformation(
                "Employee {EmployeeId} added successfully with job title {JobTitleId}.",
                employee.Id, employeeDto.JobTitleId);
        }

        // Get all job titles
        public async Task<IEnumerable<JobTitleDto>> GetAllJobTitlesAsync()
        {
            var jobTitles = await _jobTitleRepository.GetAllJobTitlesAsync();
            _logger.LogInformation("Fetched all job titles successfully.");
            return _mapper.Map<IEnumerable<JobTitleDto>>(jobTitles);
        }

        // Update an existing employee
        public async Task UpdateEmployeeAsync(EmployeeDto employeeDto)
        {
            var employee = _mapper.Map<Employee>(employeeDto);
            await _employeeRepository.UpdateEmployeeAsync(employee);
            _logger.LogInformation("Employee {EmployeeId} updated successfully.", employeeDto.Id);
        }

        // Delete an employee by ID
        public async Task DeleteEmployeeAsync(int employeeId)
        {
            await _employeeRepository.DeleteEmployeeAsync(employeeId);
            _logger.LogInformation("Employee {EmployeeId} deleted successfully.", employeeId);
        }

        // Get all employees
        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllEmployeesAsync();
            var jobTitles = await _jobTitleRepository.GetAllJobTitlesAsync();

            var employeeDtos = employees.Select(e =>
            {
                var dto = _mapper.Map<EmployeeDto>(e);
                // Employees fetched here have no JobTitle navigation loaded, so
                // resolve the display name from the separately-fetched job titles.
                dto.JobTitleName = jobTitles.FirstOrDefault(j => j.Id == e.JobTitleId)?.JobTitle;
                return dto;
            });

            return employeeDtos;
        }


        // Get employee by ID
        public async Task<EmployeeDto> GetEmployeeByIdAsync(int employeeId)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(employeeId);

            if (employee == null)
            {
                _logger.LogWarning("Employee {EmployeeId} not found.", employeeId);
                throw new NotFoundException($"Employee with ID {employeeId} was not found.");
            }

            var employeeDto = _mapper.Map<EmployeeDto>(employee);
            _logger.LogInformation("Fetched employee {EmployeeId} successfully.", employeeId);
            return employeeDto;
        }
    }
}
