using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using szpont.Models;

namespace szpont.Controllers
{
    [Authorize(Roles = "kierownik")]
    public class KierownikDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Dashboards/KierownikDashboard/Index.cshtml");
        }
    }
}

