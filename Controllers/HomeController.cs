using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using szpont.Data;
using szpont.Models;

namespace szpont.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var topicsCount = await _context.Topics.CountAsync();
            var promotors = await _userManager.GetUsersInRoleAsync("promotor");
            var promotorsCount = promotors.Count;

            ViewBag.TopicsCount = topicsCount;
            ViewBag.PromotorsCount = promotorsCount;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("admin"))
                return RedirectToAction("Index", "AdminDashboard");
            if (roles.Contains("dziekan"))
                return RedirectToAction("Index", "DziekanDashboard");
            if (roles.Contains("kierownik"))
                return RedirectToAction("Index", "KierownikDashboard");
            if (roles.Contains("promotor"))
                return RedirectToAction("Index", "PromotorDashboard");
            if (roles.Contains("student"))
                return RedirectToAction("Index", "StudentDashboard");

            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
