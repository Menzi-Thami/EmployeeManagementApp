using EmployeeManagementApp.Application.DTOs;

namespace EmployeeManagementApp.Application.Services
{
    public interface IProjectService
    {
        Task<IReadOnlyList<ProjectDto>> GetAllProjectsAsync(CancellationToken cancellationToken);
        Task<ProjectDto> GetProjectByIdAsync(int id, CancellationToken cancellationToken);
        Task UpdateProjectCostAsync(int projectId, CancellationToken cancellationToken);
    }
}
