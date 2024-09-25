using EmployeeManagementApp.Application.Services;
using EmployeeManagementApp.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmployeeManagementApp.Pages.Employee
{
    public class AddModel : PageModel
    {
        private readonly IEmployeeService _employeeService; 
        public EmployeeDto EmployeeDto { get; set; } = new EmployeeDto();
        public List<JobTitleDto> JobTitles { get; set; } = new List<JobTitleDto>();

        public AddModel(IEmployeeService employeeService) 
        {
            _employeeService = employeeService; 
        }

        public async Task OnGetAsync()
        {
            JobTitles = (List<JobTitleDto>)await _employeeService.GetAllJobTitlesAsync(); // Fetch job titles from employee service
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                JobTitles = (List<JobTitleDto>)await _employeeService.GetAllJobTitlesAsync(); 
                return Page();
            }

            // Call the employee service to save the employee
            await _employeeService.AddEmployeeAsync(EmployeeDto);

            return RedirectToPage("/Employee/Index"); 
        }
    }
}
