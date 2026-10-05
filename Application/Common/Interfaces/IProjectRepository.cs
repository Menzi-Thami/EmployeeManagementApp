using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EmployeeManagementApp.Application.Common.Models;

namespace EmployeeManagementApp.Application.Common.Interfaces
{
    public interface IProjectRepository
    {
        Task<IReadOnlyList<ProjectSummary>> GetAllProjectsAsync(CancellationToken cancellationToken);
        Task<ProjectSummary?> GetProjectByIdAsync(int id, CancellationToken cancellationToken);
        Task UpdateProjectCostAsync(int projectId, decimal cost, CancellationToken cancellationToken);
    }
}
