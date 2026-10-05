using EmployeeManagementApp.Application.DTOs;
using EmployeeManagementApp.Application.Common.Exceptions;
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
        public async Task<IReadOnlyList<ProjectDto>> GetAllProjectsAsync(CancellationToken cancellationToken)
        {
            var projects = await _projectRepository.GetAllProjectsAsync(cancellationToken);
            return projects.Select(ToDto).ToList();
        }

        // Get project by ID
        public async Task<ProjectDto> GetProjectByIdAsync(int id, CancellationToken cancellationToken)
        {
            var project = await _projectRepository.GetProjectByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"Project with ID {id} was not found.");
            return ToDto(project);
        }

        // Recalculate and store the project cost. The pricing rule lives only in IProjectCostCalculator.
        public async Task UpdateProjectCostAsync(int projectId, CancellationToken cancellationToken)
        {
            var cost = await _projectCostCalculator.CalculateProjectCostAsync(projectId, cancellationToken);
            await _projectRepository.UpdateProjectCostAsync(projectId, cost, cancellationToken);
        }
    }
}
