using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using szpont.Data;
using szpont.Models;
using System.Security.Claims;

namespace szpont.Controllers
{
    [Authorize(Roles = "promotor")]
    public class PromotorDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PromotorDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var myTopics = await _context.Topics
                .Where(t => t.PromotorId == userId)
                .ToListAsync();

            ViewBag.CountDraft = myTopics.Count(t => t.Status == TopicStatus.Draft);
            ViewBag.CountWaiting = myTopics.Count(t => t.Status == TopicStatus.WaitingForKierownik || t.Status == TopicStatus.WaitingForDziekan);
            ViewBag.CountApproved = myTopics.Count(t => t.Status == TopicStatus.Approved);
            ViewBag.CountRejected = myTopics.Count(t => t.Status == TopicStatus.Rejected);

            return View("~/Views/Dashboards/PromotorDashboard/Index.cshtml", myTopics);
        }
    }
}