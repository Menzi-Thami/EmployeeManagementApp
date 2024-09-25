using EmployeeManagementApp.Application.DTOs;

namespace EmployeeManagementApp.Application.Services
{
    public interface IProjectService
    {
        IEnumerable<ProjectDto> GetAllProjects();
        ProjectDto GetProjectById(int id);
        void UpdateProjectCost(int projectId);
        decimal CalculateProjectCost(ProjectDto project);
    }
}
