using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using szpont.Data;
using szpont.Models;

namespace szpont.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminDashboardController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new AdminDashboardViewModel();

            // Statystyki użytkowników
            var allUsers = await _userManager.Users.ToListAsync();
            viewModel.TotalUsers = allUsers.Count;

            // Statystyki studentów
            var students = await _userManager.GetUsersInRoleAsync("student");
            viewModel.TotalStudents = students.Count;

            // Statystyki innych ról
            var admins = await _userManager.GetUsersInRoleAsync("admin");
            viewModel.TotalAdmins = admins.Count;

            var promotors = await _userManager.GetUsersInRoleAsync("promotor");
            viewModel.TotalPromotors = promotors.Count;

            var dziekans = await _userManager.GetUsersInRoleAsync("dziekan");
            viewModel.TotalDziekans = dziekans.Count;

            var kierowniks = await _userManager.GetUsersInRoleAsync("kierownik");
            viewModel.TotalKierowniks = kierowniks.Count;

            // Statystyki tematów
            var allTopics = await _context.Topics.ToListAsync();
            viewModel.TotalTopics = allTopics.Count;

            // Grupowanie tematów według typu
            viewModel.TopicsByType = allTopics
                .GroupBy(t => t.Type)
                .ToDictionary(g => g.Key, g => g.Count());

            // Główne statystyki dla kart - tylko 2 karty
            viewModel.MainStats = new Dictionary<string, (int Value, string CssClass)>
            {
                { "Wszyscy użytkownicy", (viewModel.TotalUsers, "stat-card-users") },
                { "Tematy", (viewModel.TotalTopics, "stat-card-topics") }
            };

            // Szczegółowe statystyki użytkowników
            viewModel.UserRoleStats = new Dictionary<string, int>
            {
                { "Studenci", viewModel.TotalStudents },
                { "Administratorzy", viewModel.TotalAdmins },
                { "Promotorzy", viewModel.TotalPromotors },
                { "Dziekani", viewModel.TotalDziekans },
                { "Kierownicy", viewModel.TotalKierowniks }
            };

            return View("~/Views/Dashboards/AdminDashboard/Index.cshtml", viewModel);
        }
    }
}

