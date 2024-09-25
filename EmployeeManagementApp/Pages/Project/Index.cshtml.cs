using EmployeeManagementApp.Application.DTOs;
using EmployeeManagementApp.Application.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagementApp.Pages.Project
{
    public class IndexModel : PageModel
    {
        private readonly IProjectService _projectService;

        public IndexModel(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public IEnumerable<ProjectDto> Projects { get; set; }

        public void OnGetProject()
        {
            Projects = _projectService.GetAllProjects(); 
        }

    }
}
