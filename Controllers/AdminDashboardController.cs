using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using szpont.Data;
using szpont.Helpers;
using szpont.Models;
using System.Text;

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
                .ToDictionary(g => g.Key.GetDisplayName(), g => g.Count());

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

        public async Task<IActionResult> ExportSystemReportTxt()
        {
            var reportData = await GetSystemReportDataAsync();
            var content = BuildTxtReportContent(reportData);
            var bytes = new UTF8Encoding(true).GetBytes(content);
            var fileName = $"raport_systemowy_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

            return File(bytes, "text/plain; charset=utf-8", fileName);
        }

        public async Task<IActionResult> ExportSystemReportCsv()
        {
            var reportData = await GetSystemReportDataAsync();
            var content = BuildCsvReportContent(reportData);
            var bytes = new UTF8Encoding(true).GetBytes(content);
            var fileName = $"raport_systemowy_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            return File(bytes, "text/csv; charset=utf-8", fileName);
        }

        private async Task<SystemReportData> GetSystemReportDataAsync()
        {
            var data = new SystemReportData
            {
                GeneratedAt = DateTime.Now
            };

            // Statystyki użytkowników
            data.TotalUsers = await _userManager.Users.CountAsync();

            // Użytkownicy według roli
            data.UsersByRole = new Dictionary<string, int>
            {
                { "admin", (await _userManager.GetUsersInRoleAsync("admin")).Count },
                { "student", (await _userManager.GetUsersInRoleAsync("student")).Count },
                { "promotor", (await _userManager.GetUsersInRoleAsync("promotor")).Count },
                { "dziekan", (await _userManager.GetUsersInRoleAsync("dziekan")).Count },
                { "kierownik", (await _userManager.GetUsersInRoleAsync("kierownik")).Count }
            };

            // Statystyki tematów
            data.TotalTopics = await _context.Topics.CountAsync();

            // Dostępne tematy = Approved i bez przypisanego studenta
            data.AvailableTopics = await _context.Topics
                .CountAsync(t => t.Status == TopicStatus.Approved && t.StudentId == null);

            return data;
        }

        private static string BuildTxtReportContent(SystemReportData data)
        {
            var sb = new StringBuilder();

            sb.AppendLine("===========================================");
            sb.AppendLine("         RAPORT SYSTEMOWY SZPONT");
            sb.AppendLine("===========================================");
            sb.AppendLine();
            sb.AppendLine($"Data wygenerowania: {data.GeneratedAt:dd.MM.yyyy HH:mm:ss}");
            sb.AppendLine();
            sb.AppendLine("-------------------------------------------");
            sb.AppendLine("STATYSTYKI UŻYTKOWNIKÓW");
            sb.AppendLine("-------------------------------------------");
            sb.AppendLine($"Liczba użytkowników ogółem: {data.TotalUsers}");
            sb.AppendLine();
            sb.AppendLine("Użytkownicy według roli:");
            foreach (var role in data.UsersByRole)
            {
                var roleName = GetPolishRoleName(role.Key);
                sb.AppendLine($"  - {roleName}: {role.Value}");
            }
            sb.AppendLine();
            sb.AppendLine("-------------------------------------------");
            sb.AppendLine("STATYSTYKI TEMATÓW");
            sb.AppendLine("-------------------------------------------");
            sb.AppendLine($"Liczba tematów ogółem: {data.TotalTopics}");
            sb.AppendLine($"Liczba dostępnych tematów: {data.AvailableTopics}");
            sb.AppendLine();
            sb.AppendLine("===========================================");
            sb.AppendLine("          KONIEC RAPORTU");
            sb.AppendLine("===========================================");

            return sb.ToString();
        }

        private static string BuildCsvReportContent(SystemReportData data)
        {
            var sb = new StringBuilder();

            // Nagłówek
            sb.AppendLine("Kategoria,Nazwa,Wartość");

            // Data wygenerowania
            sb.AppendLine($"Raport,Data wygenerowania,{data.GeneratedAt:dd.MM.yyyy HH:mm:ss}");

            // Statystyki użytkowników
            sb.AppendLine($"Użytkownicy,Ogółem,{data.TotalUsers}");
            foreach (var role in data.UsersByRole)
            {
                var roleName = GetPolishRoleName(role.Key);
                sb.AppendLine($"Użytkownicy,{EscapeCsv(roleName)},{role.Value}");
            }

            // Statystyki tematów
            sb.AppendLine($"Tematy,Ogółem,{data.TotalTopics}");
            sb.AppendLine($"Tematy,Dostępne,{data.AvailableTopics}");

            return sb.ToString();
        }

        private static string GetPolishRoleName(string role)
        {
            return role switch
            {
                "admin" => "Administratorzy",
                "student" => "Studenci",
                "promotor" => "Promotorzy",
                "dziekan" => "Dziekani",
                "kierownik" => "Kierownicy",
                _ => role
            };
        }

        private static string EscapeCsv(string value)
        {
            var normalized = value ?? string.Empty;
            if (normalized.Contains('"'))
            {
                normalized = normalized.Replace("\"", "\"\"");
            }

            if (normalized.Contains(',') || normalized.Contains('"') || normalized.Contains('\n') || normalized.Contains('\r'))
            {
                normalized = $"\"{normalized}\"";
            }

            return normalized;
        }

        private class SystemReportData
        {
            public DateTime GeneratedAt { get; set; }
            public int TotalUsers { get; set; }
            public Dictionary<string, int> UsersByRole { get; set; } = new();
            public int TotalTopics { get; set; }
            public int AvailableTopics { get; set; }
        }
    }
}

