using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using szpont.Models;

namespace szpont.Controllers
{
    [Authorize(Roles = "student")]
    public class StudentDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Dashboards/StudentDashboard/Index.cshtml");
        }
    }
}

