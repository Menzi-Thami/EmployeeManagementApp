using EmployeeManagementApp.Application.DTOs;
using EmployeeManagementApp.Infrastructure.Interfaces;
using EmployeeManagementApp.Domain.Models;
using AutoMapper;

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

        public IEnumerable<ProjectDto> GetAllProjects()
        {
            var projects = _projectRepository.GetAllProjects();
            return _mapper.Map<IEnumerable<ProjectDto>>(projects);
        }

        public ProjectDto GetProjectById(int id)
        {
            var project = _projectRepository.GetProjectById(id);
            return _mapper.Map<ProjectDto>(project);
        }

        public void UpdateProjectCost(int projectId)
        {
            var cost = _projectCostCalculator.CalculateProjectCost(projectId);
            _projectRepository.UpdateProjectCost(projectId, cost);
        }


        public decimal CalculateProjectCost(ProjectDto project)
        {
            decimal totalCost = project.Cost; // Start with the base cost of the project

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
                    case 3: // Tester
                        totalCost += 1000;
                        break;
                    case 4: // Business Analyst
                        totalCost += 4500;
                        break;
                    default:
                        break;
                }
            }

            return totalCost;
        }
    }
}

