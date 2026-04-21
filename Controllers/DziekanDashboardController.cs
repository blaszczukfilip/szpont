using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using szpont.Data;
using szpont.Models;
using System.Security.Claims;

namespace szpont.Controllers
{
    [Authorize(Roles = "dziekan")]
    public class DziekanDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int MaxReservableTopicsPerPromotor = 10;

        public DziekanDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder)
        {
            if (string.IsNullOrEmpty(sortOrder)) 
            {
                sortOrder = "date_asc";
            }

            ViewBag.CurrentSort = sortOrder;

            var topicsQuery = _context.Topics
                .Include(t => t.Promotor)
                .Include(t => t.Kierownik)
                .Where(t => t.Status == TopicStatus.WaitingForDziekan)
                .AsQueryable();

            topicsQuery = sortOrder switch
            {
                "date_desc" => topicsQuery.OrderByDescending(t => t.SubmittedDate ?? t.CreatedDate),
                _ => topicsQuery.OrderBy(t => t.SubmittedDate ?? t.CreatedDate)
            };

            var topicsWaitingForDziekan = await topicsQuery.ToListAsync();

            return View("~/Views/Dashboards/DziekanDashboard/Index.cshtml", topicsWaitingForDziekan);
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

            if (topic.Status != TopicStatus.WaitingForDziekan)
            {
                TempData["ErrorMessage"] = "Temat nie jest w statusie oczekującym na decyzję Dziekana.";
                return RedirectToAction("Details", "Topics", new { id = topic.Id });
            }

            var reservableTopicsCount = await _context.Topics.CountAsync(t =>
                t.PromotorId == topic.PromotorId &&
                t.Status == TopicStatus.Approved &&
                t.StudentId == null);

            if (reservableTopicsCount >= MaxReservableTopicsPerPromotor)
            {
                TempData["ErrorMessage"] = $"Promotor osiągnął limit {MaxReservableTopicsPerPromotor} tematów dostępnych do rezerwacji. Nie można zatwierdzić tego tematu.";
                return RedirectToAction("Details", "Topics", new { id = topic.Id });
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            topic.Status = TopicStatus.Approved;
            topic.DziekanId = currentUserId;
            topic.ApprovedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Temat został ostatecznie zatwierdzony przez Dziekana.";
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

            if (topic.Status != TopicStatus.WaitingForDziekan)
            {
                TempData["ErrorMessage"] = "Temat nie jest w statusie oczekującym na decyzję Dziekana.";
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
            topic.DziekanId = currentUserId;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Temat został odrzucony przez Dziekana.";
            return RedirectToAction("Details", "Topics", new { id = id });
        }
    }
}

