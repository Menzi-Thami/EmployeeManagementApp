using AutoMapper;
using EmployeeManagementApp.Application.DTOs;
using EmployeeManagementApp.Domain.Models;
using EmployeeManagementApp.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using EmployeeManagementApp.Infrastructure.Repositories;

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
            try
            {
                var jobTitle = await _jobTitleRepository.GetJobTitleByIdAsync(employeeDto.JobTitleId);
                if (jobTitle != null)
                {
                    employeeDto.JobTitleName = jobTitle.JobTitle;
                }
                else
                {
                    _logger.LogWarning($"Job title with ID {employeeDto.JobTitleId} not found.");
                }

                var employee = _mapper.Map<Employee>(employeeDto);
                await _employeeRepository.AddEmployeeAsync(employee);
                _logger.LogInformation($"Employee {employee.Name} {employee.Surname} with job title {employeeDto.JobTitleName} added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding employee.");
                throw;
            }
        }

        // Get all job titles
        public async Task<IEnumerable<JobTitleDto>> GetAllJobTitlesAsync()
        {
            try
            {
                var jobTitles = await _jobTitleRepository.GetAllJobTitlesAsync();
                _logger.LogInformation("Fetched all job titles successfully.");
                return _mapper.Map<IEnumerable<JobTitleDto>>(jobTitles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching job titles.");
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
            var employees = await _employeeRepository.GetAllEmployeesAsync();
            var jobTitles = await _jobTitleRepository.GetAllJobTitlesAsync();

            var employeeDtos = employees.Select(e => new EmployeeDto
            {
                Id = e.Id,
                Name = e.Name,
                Surname = e.Surname,
                DateOfBirth = e.DateOfBirth,
                JobTitleId = e.JobTitleId,
                JobTitleName = jobTitles.FirstOrDefault(j => j.Id == e.JobTitleId)?.JobTitle 
            });

            return employeeDtos;
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
