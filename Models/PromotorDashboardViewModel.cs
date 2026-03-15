using Microsoft.AspNetCore.Mvc;

namespace szpont.Models
{
    public class PromotorDashboardViewModel : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
