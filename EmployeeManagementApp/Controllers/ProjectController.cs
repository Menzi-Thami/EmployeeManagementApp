using EmployeeManagementApp.Application.DTOs;
using EmployeeManagementApp.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementApp.Controllers
{
    [Route("[controller]")]
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var projects = _projectService.GetAllProjects(); 
            return View(projects);
        }
        [HttpGet("{id}")]
        public IActionResult Details(int id)
        {
            var project = _projectService.GetProjectById(id);
            if (project == null) return NotFound();
            return View(project); 
        }

        [HttpGet("{id}/UpdateCost")]
        public IActionResult UpdateCost(int id)
        {
            return View(id); 
        }

        [HttpPost("{id}/UpdateCost")]
        public IActionResult UpdateCost(int id, decimal newCost)
        {
            _projectService.UpdateProjectCost(id);
            return RedirectToAction("Index"); 
        }
    }
}
