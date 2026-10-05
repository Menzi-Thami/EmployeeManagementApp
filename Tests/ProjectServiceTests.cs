using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EmployeeApi.Controllers;
using EmployeeManagementApp.Application.Common.Exceptions;
using EmployeeManagementApp.Application.Common.Interfaces;
using EmployeeManagementApp.Application.Common.Models;
using EmployeeManagementApp.Application.DTOs;
using EmployeeManagementApp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Shouldly;
using Xunit;

namespace EmployeeManagementApp.UnitTests
{
    public class ProjectServiceTests
    {
        private readonly IProjectRepository _projectRepository = Substitute.For<IProjectRepository>();
        private readonly IProjectCostCalculator _costCalculator = Substitute.For<IProjectCostCalculator>();

        // A live token, so the substitutes only match when the service forwards it.
        private readonly CancellationToken _ct = new CancellationTokenSource().Token;

        private ProjectService CreateSut() => new(_projectRepository, _costCalculator);

        private static ProjectSummary Summary(int id, string? employeeNames) => new()
        {
            Id = id,
            Name = $"Project {id}",
            StartDate = new DateTime(2024, 1, 1),
            Cost = 1000m,
            EmployeeNames = employeeNames
        };

        [Fact]
        public async Task GetAllProjectsAsync_SplitsAggregatedEmployeeNames()
        {
            _projectRepository.GetAllProjectsAsync(_ct).Returns([Summary(1, "Grace Hopper, Alan Turing")]);

            var project = (await CreateSut().GetAllProjectsAsync(_ct)).Single();

            project.EmployeeNames.ShouldBe(["Grace Hopper", "Alan Turing"]);
        }

        [Fact]
        public async Task GetAllProjectsAsync_WhenProjectHasNoEmployees_ReturnsEmptyNameList()
        {
            _projectRepository.GetAllProjectsAsync(_ct).Returns([Summary(2, null)]);

            var project = (await CreateSut().GetAllProjectsAsync(_ct)).Single();

            project.EmployeeNames.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetProjectByIdAsync_WhenProjectDoesNotExist_ThrowsNotFoundException()
        {
            _projectRepository.GetProjectByIdAsync(42, _ct).Returns((ProjectSummary?)null);

            var ex = await Should.ThrowAsync<NotFoundException>(() => CreateSut().GetProjectByIdAsync(42, _ct));

            ex.Message.ShouldContain("42");
        }

        [Fact]
        public async Task UpdateProjectCostAsync_StoresTheCalculatorResult()
        {
            _costCalculator.CalculateProjectCostAsync(7, _ct).Returns(5500m);

            await CreateSut().UpdateProjectCostAsync(7, _ct);

            await _projectRepository.Received(1).UpdateProjectCostAsync(7, 5500m, _ct);
        }

        [Fact]
        public async Task HomeControllerViewProjects_PassesServiceDtosToTheViewWithEmployeeNames()
        {
            var projectService = Substitute.For<IProjectService>();
            IReadOnlyList<ProjectDto> dtos =
            [
                new() { Id = 1, Name = "Apollo", EmployeeNames = ["Grace Hopper", "Alan Turing"] }
            ];
            projectService.GetAllProjectsAsync(_ct).Returns(dtos);
            var controller = new HomeController(
                NullLogger<HomeController>.Instance, Substitute.For<IEmployeeService>(), projectService);

            var result = (await controller.ViewProjects(_ct)).ShouldBeOfType<ViewResult>();

            var model = result.Model.ShouldBeAssignableTo<IEnumerable<ProjectDto>>()!.ToList();
            model.Single().EmployeeNames.ShouldBe(["Grace Hopper", "Alan Turing"]);
        }
    }
}
