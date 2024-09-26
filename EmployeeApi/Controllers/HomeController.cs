using EmployeeApi.Models;
using EmployeeManagementApp.Application.DTOs;
using EmployeeManagementApp.Application.Services;
using EmployeeManagementApp.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EmployeeApi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEmployeeService _employeeService;
        private readonly IProjectService _projectService;

        public HomeController(ILogger<HomeController> logger, IEmployeeService employeeService, IProjectService projectService)
        {
            _logger = logger;
            _employeeService = employeeService;
            _projectService = projectService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: /addemploy
        public IActionResult AddEmployee()
        {
            return View();
        }

        // POST: /addemploy
        [HttpPost]
        public async Task<IActionResult> AddEmployee(EmployeeDto employeeDto)
        {
            if (ModelState.IsValid)
            {
                await _employeeService.AddEmployeeAsync(employeeDto);
                return RedirectToAction("ViewEmployees");
            }

            return View(employeeDto);
        }

        // GET: /viewemployees
        public async Task<IActionResult> ViewEmployees()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return View(employees);
        }

        // GET: /employeelist
        public async Task<IActionResult> EmployeeList()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return View(employees);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // GET: /viewprojects
        public IActionResult ViewProjects()
        {
            var projects = _projectService.GetAllProjects();

            if (projects == null || !projects.Any())
            {
                return View(new List<ProjectDto>());
            }

            var viewModel = projects.Select(p => new ProjectDto
            {
                Id = p.Id,
                Name = p.Name,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Cost = p.Cost,
                EmployeeNames = p.Employees?.Select(e => $"{e.Name} {e.Surname}").ToList() ?? new List<string>()
            }).ToList();

            return View(viewModel);
        }
    }
}
