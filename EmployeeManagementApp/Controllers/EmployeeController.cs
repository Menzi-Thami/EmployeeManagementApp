using Microsoft.AspNetCore.Mvc;
using EmployeeManagementApp.Application.Services;
using EmployeeManagementApp.Application.DTOs;

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
        public IActionResult AddEmployee(EmployeeDto employeeDto)
        {
            if (ModelState.IsValid)
            {
                _employeeService.AddEmployeeAsync(employeeDto);
                return RedirectToAction("ViewEmployees");
            }

            return View(employeeDto);
        }

        // GET: /viewemployees
        public IActionResult ViewEmployees()
        {
            // Retrieve all employees using the employee service
            var employees = _employeeService.GetAllEmployeesAsync().Result;

            return View(employees); 
        }
    }
}
