using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using szpont.Models;

namespace szpont.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Dashboards/AdminDashboard/Index.cshtml");
        }
    }
}

