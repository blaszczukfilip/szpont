using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [Authorize(Roles = RoleNames.Promotor)]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Promotor)]
        public async Task<IActionResult> Create([Bind("Title,Description,Type,Keywords")] Topic model)
        {
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
    }
}
