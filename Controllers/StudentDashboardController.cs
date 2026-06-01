using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using szpont.Data;
using szpont.Models;
using szpont.Services;

namespace szpont.Controllers
{
    [Authorize(Roles = "student")]
    public class StudentDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public StudentDashboardController(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(currentUserId))
            {
                ViewBag.Notifications = await _notificationService.GetUserNotificationsAsync(currentUserId);
                ViewBag.UnreadNotificationsCount = await _notificationService.GetUnreadCountAsync(currentUserId);
            }

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
