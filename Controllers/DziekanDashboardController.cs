using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using szpont.Models;

namespace szpont.Controllers
{
    [Authorize(Roles = "dziekan")]
    public class DziekanDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Dashboards/DziekanDashboard/Index.cshtml");
        }
    }
}

