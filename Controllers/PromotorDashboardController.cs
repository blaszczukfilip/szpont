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
            ViewBag.PendingReservationsCount = myTopics.Count(t =>
                t.StudentId != null && t.ReservationStatus == ReservationStatus.Pending);

            return View("~/Views/Dashboards/PromotorDashboard/Index.cshtml", myTopics);
        }

        public async Task<IActionResult> Reservations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pending = await _context.Topics
                .Include(t => t.Student)
                .Where(t => t.PromotorId == userId && t.StudentId != null && t.ReservationStatus == ReservationStatus.Pending)
                .OrderByDescending(t => t.ReservationDate)
                .ToListAsync();

            return View("~/Views/Dashboards/PromotorDashboard/Reservations.cshtml", pending);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptReservation(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var topic = await _context.Topics.FindAsync(id);

            if (topic == null)
                return NotFound();
            if (topic.PromotorId != userId)
                return Forbid();
            if (topic.ReservationStatus != ReservationStatus.Pending)
            {
                TempData["ErrorMessage"] = "Ta rezerwacja nie oczekuje na akceptację.";
                return RedirectToAction("Details", "Topics", new { id = topic.Id });
            }

            topic.ReservationStatus = ReservationStatus.Accepted;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Rezerwacja została zaakceptowana. Student jest przypisany do tematu.";
            return RedirectToAction("Details", "Topics", new { id = topic.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectReservation(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var topic = await _context.Topics.FindAsync(id);

            if (topic == null)
                return NotFound();
            if (topic.PromotorId != userId)
                return Forbid();
            if (topic.ReservationStatus != ReservationStatus.Pending)
            {
                TempData["ErrorMessage"] = "Ta rezerwacja nie oczekuje na decyzję.";
                return RedirectToAction("Details", "Topics", new { id = topic.Id });
            }

            topic.StudentId = null;
            topic.ReservationDate = null;
            topic.ReservationStatus = null;
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Rezerwacja została odrzucona. Temat jest ponownie dostępny dla studentów.";
            return RedirectToAction("Details", "Topics", new { id = topic.Id });
        }
    }
}