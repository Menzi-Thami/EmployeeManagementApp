using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagementApp.Application.Common.Exceptions;
using EmployeeManagementApp.Application.Common.Interfaces;
using EmployeeManagementApp.Application.DTOs;
using EmployeeManagementApp.Application.Services;
using EmployeeManagementApp.Domain.Models;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace EmployeeManagementApp.UnitTests
{
    public class EmployeeServiceTests
    {
        private readonly IEmployeeRepository _employeeRepository = Substitute.For<IEmployeeRepository>();
        private readonly IJobTitleRepository _jobTitleRepository = Substitute.For<IJobTitleRepository>();
        private readonly ILogger<EmployeeService> _logger = Substitute.For<ILogger<EmployeeService>>();

        private EmployeeService CreateSut() =>
            new EmployeeService(_employeeRepository, _jobTitleRepository, _logger);

        [Fact]
        public async Task GetEmployeeByIdAsync_WhenEmployeeDoesNotExist_ThrowsNotFoundException()
        {
            _employeeRepository.GetEmployeeByIdAsync(42).Returns((Employee?)null);
            var sut = CreateSut();

            var ex = await Should.ThrowAsync<NotFoundException>(() => sut.GetEmployeeByIdAsync(42));
            ex.Message.ShouldContain("42");
        }

        [Fact]
        public async Task GetEmployeeByIdAsync_WhenEmployeeExists_ReturnsCorrectlyMappedDto()
        {
            var employee = new Employee
            {
                Id = 7,
                Name = "Ada",
                Surname = "Lovelace",
                JobTitleId = 1,
                JobTitle = new JobTitles { Id = 1, JobTitle = "Developer" },
                DateOfBirth = new DateTime(1990, 5, 1)
            };
            _employeeRepository.GetEmployeeByIdAsync(7).Returns(employee);
            var sut = CreateSut();

            var dto = await sut.GetEmployeeByIdAsync(7);

            dto.ShouldNotBeNull();
            dto.Id.ShouldBe(7);
            dto.Name.ShouldBe("Ada");
            dto.Surname.ShouldBe("Lovelace");
            dto.JobTitleId.ShouldBe(1);
            dto.JobTitleName.ShouldBe("Developer");
            dto.DateOfBirth.ShouldBe(new DateTime(1990, 5, 1));
        }

        [Fact]
        public async Task GetAllEmployeesAsync_MapsEmployeesAndResolvesJobTitleNameFromLookup()
        {
            var employees = new List<Employee>
            {
                new Employee { Id = 1, Name = "Grace", Surname = "Hopper", JobTitleId = 2, DateOfBirth = new DateTime(1980, 1, 1) },
                new Employee { Id = 2, Name = "Alan", Surname = "Turing", JobTitleId = 3, DateOfBirth = new DateTime(1975, 6, 23) }
            };
            var jobTitles = new List<JobTitles>
            {
                new JobTitles { Id = 2, JobTitle = "DBA" },
                new JobTitles { Id = 3, JobTitle = "QA" }
            };
            _employeeRepository.GetAllEmployeesAsync().Returns(employees);
            _jobTitleRepository.GetAllJobTitlesAsync().Returns(jobTitles);
            var sut = CreateSut();

            var result = (await sut.GetAllEmployeesAsync()).ToList();

            result.Count.ShouldBe(2);

            var grace = result.Single(e => e.Id == 1);
            grace.Name.ShouldBe("Grace");
            grace.Surname.ShouldBe("Hopper");
            grace.JobTitleId.ShouldBe(2);
            grace.JobTitleName.ShouldBe("DBA");

            var alan = result.Single(e => e.Id == 2);
            alan.Name.ShouldBe("Alan");
            alan.JobTitleName.ShouldBe("QA");
        }

        [Fact]
        public async Task GetAllJobTitlesAsync_ReturnsOneDtoPerJobTitle()
        {
            var jobTitles = new List<JobTitles>
            {
                new JobTitles { Id = 1, JobTitle = "Developer" },
                new JobTitles { Id = 4, JobTitle = "Business Analyst" }
            };
            _jobTitleRepository.GetAllJobTitlesAsync().Returns(jobTitles);
            var sut = CreateSut();

            var result = (await sut.GetAllJobTitlesAsync()).ToList();

            result.Count.ShouldBe(2);
            result.Select(j => j.Id).ShouldBe(new[] { 1, 4 }, ignoreOrder: true);
        }
    }
}
