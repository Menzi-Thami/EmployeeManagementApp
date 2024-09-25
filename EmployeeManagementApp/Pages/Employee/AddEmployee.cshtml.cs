using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EmployeeManagementApp.Application.Services; 
using EmployeeManagementApp.Application.DTOs; 

namespace EmployeeManagementApp.API.Pages.Employee
{
    public class AddEmployeeModel : PageModel
    {
        private readonly IEmployeeService _employeeService; 

        [BindProperty] 
        public EmployeeDto Employee { get; set; } = new EmployeeDto(); 

        public AddEmployeeModel(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        public void OnGet()
        {
            _employeeService.GetAllJobTitlesAsync();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) 
            {
                return Page(); 
            }

            _employeeService.AddEmployeeAsync(Employee); 

            return RedirectToPage("ViewProjects"); 
        }
    }
}
