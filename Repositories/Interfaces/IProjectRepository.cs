using System.Collections.Generic;
using EmployeeManagementApp.Domain.Models;

namespace EmployeeManagementApp.Infrastructure.Interfaces
{
    public interface IProjectRepository
    {
        IEnumerable<Project> GetAllProjects();
        Project GetProjectById(int id);
        void UpdateProjectCost(int projectId, decimal cost);
    }
}
