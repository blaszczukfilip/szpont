using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using szpont.Models;

namespace szpont.Controllers
{
    [Authorize(Roles = "promotor")]
    public class PromotorDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View("~/Views/Dashboards/PromotorDashboard/Index.cshtml");
        }
    }
}

