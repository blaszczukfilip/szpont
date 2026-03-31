using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using szpont.Data;
using szpont.Models;

namespace szpont.Controllers
{
    public class TopicsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TopicsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchTerm, string typeFilter, string keywordFilter)
        {
            var topics = _context.Topics.AsQueryable();


            // filtrowanie po tekscie (tytul/opis)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                topics = topics.Where(t => t.Title.Contains(searchTerm) || t.Description.Contains(searchTerm));
            }

            // filtrotwanie po typie
            if (!string.IsNullOrWhiteSpace(typeFilter))
            {
                topics = topics.Where(t => t.Type == typeFilter);
            }

            // filtrowanie po keywordach
            if (!string.IsNullOrWhiteSpace(keywordFilter))
            {
                topics = topics.Where(t => t.Keywords.Contains(keywordFilter));
            }

            // pobranie unikalnych typow dla filtra do dropdownu w widoku
            var types = _context.Topics.Select(t => t.Type).Distinct().OrderBy(t => t).ToList();

            // dane przekazywane do widoku
            ViewBag.SearchTerm = searchTerm;
            ViewBag.TypeFilter = typeFilter;
            ViewBag.KeywordFilter = keywordFilter;
            ViewBag.Types = types;

            // lista tematow posortowana po dacie
            var topicsList = topics.OrderByDescending(t => t.CreatedDate).ToList();
            return View(topicsList);
        }
        [Authorize(Roles = "promotor, admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var topic = await _context.Topics
                .FirstOrDefaultAsync(m => m.Id == id);
            if (topic == null)
            {
                return NotFound();
            }
            return View(topic);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "promotor, admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            if (topic != null)
            {
                _context.Topics.Remove(topic);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var topic = await _context.Topics
                .Include(t => t.Promotor)
                .Include(t => t.Kierownik)
                .Include(t => t.Dziekan)
                .Include(t => t.Student)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (topic == null)
            {
                return NotFound();
            }
            return View(topic);
        }

        [Authorize(Roles = "promotor, admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var topic = await _context.Topics.FindAsync(id);
            if (topic == null)
                return NotFound();

            return View(topic);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "promotor, admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Type,Description,Keywords")] Topic topic)
        {
            if (id != topic.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(topic); 
                await _context.SaveChangesAsync(); 
                return RedirectToAction(nameof(Index));
            }
            return View(topic);
        }

        [Authorize(Roles = "promotor, admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "promotor, admin")]
        public async Task<IActionResult> Create([Bind("Title,Description,Type,Keywords")] Topic model)
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            model.PromotorId = userId;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.CreatedDate = DateTime.Now;
            _context.Topics.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Nowy temat został dodany.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "promotor")]
        public async Task<IActionResult> SubmitForApproval(int id)
        {
            var topic = await _context.Topics.FindAsync(id);
            var userId = System.Security.Claims.ClaimTypes.NameIdentifier;
            var currentUserId = User.FindFirst(userId)?.Value;

            if (topic == null) return NotFound();
            if (topic.PromotorId != currentUserId) return Forbid();

            if (topic.Status != TopicStatus.Draft)
            {
                TempData["ErrorMessage"] = "Tylko szkice mogą być wysłane do akceptacji.";
                return RedirectToAction(nameof(Details), new { id = topic.Id });
            }

            topic.Status = TopicStatus.WaitingForKierownik;
            topic.SubmittedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Temat został wysłany do akceptacji przez Kierownika.";

            return RedirectToAction(nameof(Details), new { id = topic.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "student")]
        public async Task<IActionResult> Reserve(int id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingReservation = await _context.Topics
                .FirstOrDefaultAsync(t => t.StudentId == currentUserId);

            if (existingReservation != null)
            {
                TempData["ErrorMessage"] = "Masz już zarezerwowany temat. Anuluj obecną rezerwację, aby zarezerwować inny.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var topic = await _context.Topics.FindAsync(id);

            if (topic == null)
                return NotFound();

            if (topic.Status != TopicStatus.Approved)
            {
                TempData["ErrorMessage"] = "Można rezerwować tylko zatwierdzone tematy.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (topic.StudentId != null)
            {
                TempData["ErrorMessage"] = "Ten temat jest już zarezerwowany przez innego studenta.";
                return RedirectToAction(nameof(Details), new { id });
            }

            topic.StudentId = currentUserId;
            topic.ReservationDate = DateTime.Now;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Temat został zarezerwowany pomyślnie.";

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "student")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var topic = await _context.Topics.FindAsync(id);

            if (topic == null)
                return NotFound();

            if (topic.StudentId != currentUserId)
            {
                TempData["ErrorMessage"] = "Nie możesz anulować rezerwacji, która nie należy do Ciebie.";
                return RedirectToAction(nameof(Details), new { id });
            }

            topic.StudentId = null;
            topic.ReservationDate = null;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Rezerwacja została anulowana.";

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
