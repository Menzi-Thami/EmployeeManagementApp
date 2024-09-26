using EmployeeManagementApp.Application.DTOs;
using EmployeeManagementApp.Infrastructure.Interfaces;
using EmployeeManagementApp.Domain.Models;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace EmployeeManagementApp.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectCostCalculator _projectCostCalculator;
        private readonly IMapper _mapper;

        public ProjectService(IProjectRepository projectRepository, IProjectCostCalculator projectCostCalculator, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _projectCostCalculator = projectCostCalculator;
            _mapper = mapper;
        }

        // Get all projects
        public IEnumerable<ProjectDto> GetAllProjects()
        {
            var projects = _projectRepository.GetAllProjects();
            return projects == null ? Enumerable.Empty<ProjectDto>() : _mapper.Map<IEnumerable<ProjectDto>>(projects);
        }


        // Get project by ID
        public ProjectDto GetProjectById(int id)
        {
            var project = _projectRepository.GetProjectById(id);
            return _mapper.Map<ProjectDto>(project);
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
