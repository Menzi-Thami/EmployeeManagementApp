using EmployeeManagementApp.Application.DTOs;
using EmployeeManagementApp.Application.Common.Interfaces;
using EmployeeManagementApp.Application.Common.Models;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeManagementApp.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectCostCalculator _projectCostCalculator;

        public ProjectService(IProjectRepository projectRepository, IProjectCostCalculator projectCostCalculator)
        {
            _projectRepository = projectRepository;
            _projectCostCalculator = projectCostCalculator;
        }

        // The repository aggregates the names in SQL (STRING_AGG); split them once here.
        private static ProjectDto ToDto(ProjectSummary project) => new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Cost = project.Cost,
            EmployeeNames = project.EmployeeNames?
                .Split(ProjectSummary.EmployeeNameSeparator, StringSplitOptions.RemoveEmptyEntries)
                .ToList() ?? []
        };

        // Get all projects
        public IEnumerable<ProjectDto> GetAllProjects()
        {
            return _projectRepository.GetAllProjects().Select(ToDto).ToList();
        }


        // Get project by ID
        public ProjectDto? GetProjectById(int id)
        {
            var project = _projectRepository.GetProjectById(id);
            return project == null ? null : ToDto(project);
        }

        // Update project cost
        public void UpdateProjectCost(int projectId)
        {
            var cost = _projectCostCalculator.CalculateProjectCost(projectId);
            _projectRepository.UpdateProjectCost(projectId, cost);
        }

        // Calculate project cost
        public decimal CalculateProjectCost(ProjectDto project)
        {
            decimal totalCost = project.Cost;

            foreach (var employee in project.Employees)
            {
                switch (employee.JobTitleId)
                {
                    case 1: // Developer
                        totalCost += 2500;
                        break;
                    case 2: // DBA
                        totalCost += 3000;
                        break;
                    case 3: // QA
                        totalCost += 1000;
                        break;
                    case 4: // Business Analyst
                        totalCost += 4500;
                        break;
                }
            }

            return totalCost;
        }
    }
}
