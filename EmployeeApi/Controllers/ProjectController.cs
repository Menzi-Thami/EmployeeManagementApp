using Microsoft.AspNetCore.Mvc;
using EmployeeManagementApp.Domain.Models;
using System.Linq;
using EmployeeManagementApp.Application.Services;
using EmployeeManagementApp.Application.DTOs;

namespace EmployeeManagementApp.API.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }
        public IActionResult Index()
        {
            return View();
        }

        // GET: /viewprojects
        public IActionResult ViewProjects()
        {
            var projects = _projectService.GetAllProjects();

            if (projects == null || !projects.Any())
            {
                // Handle the null or empty case
                projects = new List<ProjectDto>(); 
            }

            var viewModel = projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Cost = p.Cost,
                EmployeeNames = p.Employees.Select(e => $"{e.Name} {e.Surname}").ToList()
            }).ToList();

            return View(viewModel);
        }
    }
}