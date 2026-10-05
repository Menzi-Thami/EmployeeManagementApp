using System;
using System.Collections.Generic;
using System.Linq;
using EmployeeApi.Controllers;
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
        public void GetAllProjects_SplitsAggregatedEmployeeNames()
        {
            _projectRepository.GetAllProjects().Returns([Summary(1, "Grace Hopper, Alan Turing")]);

            var project = CreateSut().GetAllProjects().Single();

            project.EmployeeNames.ShouldBe(["Grace Hopper", "Alan Turing"]);
        }

        [Fact]
        public void GetAllProjects_WhenProjectHasNoEmployees_ReturnsEmptyNameList()
        {
            _projectRepository.GetAllProjects().Returns([Summary(2, null)]);

            var project = CreateSut().GetAllProjects().Single();

            project.EmployeeNames.ShouldBeEmpty();
        }

        [Fact]
        public void HomeControllerViewProjects_PassesServiceDtosToTheViewWithEmployeeNames()
        {
            var projectService = Substitute.For<IProjectService>();
            var dtos = new List<ProjectDto>
            {
                new() { Id = 1, Name = "Apollo", EmployeeNames = ["Grace Hopper", "Alan Turing"] }
            };
            projectService.GetAllProjects().Returns(dtos);
            var controller = new HomeController(
                NullLogger<HomeController>.Instance, Substitute.For<IEmployeeService>(), projectService);

            var result = controller.ViewProjects().ShouldBeOfType<ViewResult>();

            var model = result.Model.ShouldBeAssignableTo<IEnumerable<ProjectDto>>()!.ToList();
            model.Single().EmployeeNames.ShouldBe(["Grace Hopper", "Alan Turing"]);
        }
    }
}
