using System.Collections.Generic;
using EmployeeManagementApp.Application.Common.Models;

namespace EmployeeManagementApp.Application.Common.Interfaces
{
    public interface IProjectRepository
    {
        IEnumerable<ProjectSummary> GetAllProjects();
        ProjectSummary? GetProjectById(int id);
        void UpdateProjectCost(int projectId, decimal cost);
    }
}
