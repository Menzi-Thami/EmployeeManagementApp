using Microsoft.AspNetCore.Mvc;
using EmployeeManagementApp.Application.Services;
using EmployeeManagementApp.Application.DTOs;
using System.Threading.Tasks;

namespace EmployeeManagementApp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
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
            try
            {
                // Retrieve all employees asynchronously using the employee service
                var employees = await _employeeService.GetAllEmployeesAsync();
                return View(employees);
            }
            catch (Exception ex)
            {
                // Handle error (you can replace this with actual logging)
                ModelState.AddModelError(string.Empty, "An error occurred while retrieving the employees.");
                return View(new List<EmployeeDto>()); // Return an empty list in case of an error
            }
        }
    }
}
