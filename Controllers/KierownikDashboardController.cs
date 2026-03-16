using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using szpont.Data;
using szpont.Models;
using System.Security.Claims;

namespace szpont.Controllers
{
    [Authorize(Roles = "kierownik")]
    public class KierownikDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KierownikDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder)
        {
            if (string.IsNullOrEmpty(sortOrder)) //domyslnie sortuj od najstarszych
            {
                sortOrder = "date_asc";
            }

            ViewBag.CurrentSort = sortOrder;

            var topicsQuery = _context.Topics
                .Include(t => t.Promotor)
                .Where(t => t.Status == TopicStatus.WaitingForKierownik)
                .AsQueryable();

            topicsQuery = sortOrder switch
            {
                "date_desc" => topicsQuery.OrderByDescending(t => t.SubmittedDate ?? t.CreatedDate),
                _ => topicsQuery.OrderBy(t => t.SubmittedDate ?? t.CreatedDate)
            };

            var topicsWaitingForKierownik = await topicsQuery.ToListAsync();

            return View("~/Views/Dashboards/KierownikDashboard/Index.cshtml", topicsWaitingForKierownik);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound();
            }

            if (topic.Status != TopicStatus.WaitingForKierownik)
            {
                TempData["ErrorMessage"] = "Temat nie jest w statusie oczekującym na decyzję Kierownika.";
                return RedirectToAction("Details", "Topics", new { id = topic.Id });
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            topic.Status = TopicStatus.WaitingForDziekan;
            topic.KierownikId = currentUserId;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Temat został zaakceptowany przez Kierownika i przekazany do Dziekana.";
            return RedirectToAction("Details", "Topics", new { id = topic.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string reason)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
            {
                return NotFound();
            }

            if (topic.Status != TopicStatus.WaitingForKierownik)
            {
                TempData["ErrorMessage"] = "Temat nie jest w statusie oczekującym na decyzję Kierownika.";
                return RedirectToAction("Details", "Topics", new { id = id });
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                TempData["ErrorMessage"] = "Podaj powód odrzucenia tematu.";
                return RedirectToAction("Details", "Topics", new { id = id });
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            topic.Status = TopicStatus.Rejected;
            topic.RejectionReason = reason;
            topic.KierownikId = currentUserId;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Temat został odrzucony przez Kierownika.";
            return RedirectToAction("Details", "Topics", new { id = id });
        }
    }
}

