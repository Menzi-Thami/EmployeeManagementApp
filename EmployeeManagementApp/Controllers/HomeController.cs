using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementApp.API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
