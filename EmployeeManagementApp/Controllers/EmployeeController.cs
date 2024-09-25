using EmployeeManagementApp.Application.DTOs;
using EmployeeManagementApp.Application.Services;
using EmployeeManagementApp.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagementApp.Controllers
{
    [Route("[controller]")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // GET: /Employee
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return View(employees);
        }

        // GET: /Employee/Add
        [HttpGet("Add")]
        public async Task<IActionResult> AddEmployee()
        {
            var jobTitles = await _employeeService.GetAllJobTitlesAsync(); 
            ViewBag.JobTitles = jobTitles; 
            return View();
        }

        // POST: /Employee/Add
        [HttpPost("Add")]
        public async Task<IActionResult> AddEmployee(EmployeeDto employeeDto)
        {
            if (ModelState.IsValid)
            {
                await _employeeService.AddEmployeeAsync(employeeDto);
                return RedirectToAction("Index");
            }

            var jobTitles = await _employeeService.GetAllJobTitlesAsync(); 
            ViewBag.JobTitles = jobTitles;
            return View(employeeDto);
        }

        // GET: /Employee/Details/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        // GET: /Employee/Edit/{id}
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null) return NotFound();

            var jobTitles = await _employeeService.GetAllJobTitlesAsync(); 
            ViewBag.JobTitles = jobTitles;

            return View(employee);
        }

        // POST: /Employee/Edit/{id}
        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, EmployeeDto employeeDto)
        {
            if (id != employeeDto.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                await _employeeService.UpdateEmployeeAsync(employeeDto);
                return RedirectToAction("Index");
            }

            var jobTitles = await _employeeService.GetAllJobTitlesAsync(); 
            ViewBag.JobTitles = jobTitles;
            return View(employeeDto);
        }

        // GET: /Employee/Delete/{id}
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        // POST: /Employee/Delete/{id}
        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _employeeService.DeleteEmployeeAsync(id);
            return RedirectToAction("Index");
        }
    }
}
