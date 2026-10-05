using EmployeeApi.Models;
using EmployeeManagementApp.Application.Common.Exceptions;
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
        public async Task<IActionResult> AddEmployee(EmployeeDto employeeDto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return View(employeeDto);
            }

            try
            {
                await _employeeService.AddEmployeeAsync(employeeDto, cancellationToken);
            }
            catch (ValidationException ex)
            {
                // A business-rule failure belongs on the form, next to the field, not on an error page.
                foreach (var (field, messages) in ex.Errors)
                {
                    foreach (var message in messages)
                    {
                        ModelState.AddModelError(field, message);
                    }
                }

                return View(employeeDto);
            }

            return RedirectToAction("ViewEmployees");
        }

        // GET: /viewemployees
        public async Task<IActionResult> ViewEmployees(CancellationToken cancellationToken)
        {
            var employees = await _employeeService.GetAllEmployeesAsync(cancellationToken);
            return View(employees);
        }

        // GET: /employeelist
        public async Task<IActionResult> EmployeeList(CancellationToken cancellationToken)
        {
            var employees = await _employeeService.GetAllEmployeesAsync(cancellationToken);
            return View(employees);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // GET: /viewprojects
        public async Task<IActionResult> ViewProjects(CancellationToken cancellationToken)
        {
            var projects = await _projectService.GetAllProjectsAsync(cancellationToken);
            return View(projects);
        }
    }
}
