using System.Collections.Generic;
using EmployeeManagementApp.Application.Common.Exceptions;
using EmployeeManagementApp.Application.Common.Interfaces;
using EmployeeManagementApp.Application.Services;
using EmployeeManagementApp.Domain.Models;
using NSubstitute;
using Shouldly;
using Xunit;

namespace EmployeeManagementApp.UnitTests
{
    public class ProjectServiceTests
    {
        private readonly IProjectRepository _projectRepository = Substitute.For<IProjectRepository>();
        private readonly IProjectCostCalculator _costCalculator = Substitute.For<IProjectCostCalculator>();

        private ProjectService CreateSut() => new ProjectService(_projectRepository, _costCalculator);

        [Fact]
        public void GetProjectById_WhenProjectDoesNotExist_ThrowsNotFoundException()
        {
            _projectRepository.GetProjectById(42).Returns((Project?)null);
            var sut = CreateSut();

            var ex = Should.Throw<NotFoundException>(() => sut.GetProjectById(42));
            ex.Message.ShouldContain("42");
        }

        [Fact]
        public void GetProjectById_WhenProjectExists_MapsTheProject()
        {
            _projectRepository.GetProjectById(1).Returns(new Project { Id = 1, Name = "Apollo" });
            var sut = CreateSut();

            var dto = sut.GetProjectById(1);

            dto.Id.ShouldBe(1);
            dto.Name.ShouldBe("Apollo");
        }

        // The mapping deliberately leaves Employees null, so anything reading it
        // without a guard throws. ProjectController.ViewProjects did exactly that.
        // EmployeeNames is the populated field, and must stay populated.
        [Fact]
        public void GetAllProjects_PopulatesEmployeeNames_AndLeavesEmployeesNull()
        {
            var project = new Project
            {
                Id = 1,
                Name = "Apollo",
                ProjectEmployees = new List<ProjectEmployee>
                {
                    new ProjectEmployee
                    {
                        Employee = new Employee
                        {
                            Name = "Ada",
                            Surname = "Lovelace",
                            JobTitleId = 1,
                            JobTitle = new JobTitles { Id = 1, JobTitle = "Developer" }
                        }
                    }
                }
            };
            _projectRepository.GetAllProjects().Returns(new List<Project> { project });
            var sut = CreateSut();

            var dto = sut.GetAllProjects().ShouldHaveSingleItem();

            dto.EmployeeNames.ShouldBe(new[] { "Ada Lovelace" });
            dto.Employees.ShouldBeNull();
        }

        [Fact]
        public void GetAllProjects_WhenRepositoryReturnsNothing_ReturnsEmpty()
        {
            _projectRepository.GetAllProjects().Returns(new List<Project>());
            var sut = CreateSut();

            sut.GetAllProjects().ShouldBeEmpty();
        }

        // Cost belongs to IProjectCostCalculator, which reads the database. The
        // service must delegate rather than carry a second, divergent rate table.
        [Fact]
        public void UpdateProjectCost_DelegatesToTheCalculator_AndPersistsTheResult()
        {
            _costCalculator.CalculateProjectCost(7).Returns(6_000m);
            var sut = CreateSut();

            sut.UpdateProjectCost(7);

            _projectRepository.Received(1).UpdateProjectCost(7, 6_000m);
        }
    }
}
