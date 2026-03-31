using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using szpont.Data;
using szpont.Models;

namespace szpont.Controllers
{
    [Authorize(Roles = "student")]
    public class StudentDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var myReservation = await _context.Topics
                .Include(t => t.Promotor)
                .FirstOrDefaultAsync(t => t.StudentId == currentUserId);

            var availableTopics = await _context.Topics
                .CountAsync(t => t.Status == TopicStatus.Approved && t.StudentId == null);
            
            ViewBag.AvailableTopicsCount = availableTopics;

            return View("~/Views/Dashboards/StudentDashboard/Index.cshtml", myReservation);
        }
    }
}
