using EmployeeManagementApp.Application.DTOs;
using EmployeeManagementApp.Application.Common.Interfaces;
using EmployeeManagementApp.Domain.Models;
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

        // Manual mapping (replaces the old MappingProfile).
        private static ProjectDto ToDto(Project project) => new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Cost = project.Cost,
            EmployeeNames = project.ProjectEmployees?
                .Select(pe => $"{pe.Employee.Name} {pe.Employee.Surname}")
                .ToList(),
            JobTitles = project.ProjectEmployees?
                .Select(pe => new JobTitleDto
                {
                    Id = pe.Employee.JobTitleId,
                    JobTitleName = pe.Employee.JobTitle.JobTitle
                })
                .ToList()
            // Employees (List<ProjectEmployeeDto>) left null: the original
            // Project->ProjectDto map had no configuration for it (no matching source
            // member), so it was never populated.
        };

        // Get all projects
        public IEnumerable<ProjectDto> GetAllProjects()
        {
            var projects = _projectRepository.GetAllProjects();
            return projects == null ? Enumerable.Empty<ProjectDto>() : projects.Select(ToDto).ToList();
        }


        // Get project by ID
        public ProjectDto GetProjectById(int id)
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
