using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System.Security.Claims;
using System.Text;
using szpont.Data;
using szpont.Models;
using szpont.Services;

namespace szpont.Controllers
{
    public class TopicsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        private const int MaxReservableTopicsPerPromotor = 10;

        public TopicsController(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public IActionResult Index(string searchTerm, string typeFilter, string keywordFilter)
        {
            var baseTopics = _context.Topics.AsQueryable();
            //jesli user to student pokazuj tylko tematy approved
            if (User.IsInRole("student"))
            {
             baseTopics = baseTopics.Where(t => t.Status == TopicStatus.Approved && t.StudentId == null);
            }

            var types = baseTopics
                .Select(t => t.Type)
                .Distinct()
                .OrderBy(t => t)
                .ToList();

            var topics = baseTopics;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                topics = topics.Where(t => t.Title.Contains(searchTerm) || t.Description.Contains(searchTerm));
            }
            if (!string.IsNullOrWhiteSpace(typeFilter))
            {
                topics = topics.Where(t => t.Type == typeFilter);
            }
            if (!string.IsNullOrWhiteSpace(keywordFilter))
            {
                topics = topics.Where(t => t.Keywords.Contains(keywordFilter));
            }

            ViewBag.SearchTerm = searchTerm;
            ViewBag.TypeFilter = typeFilter;
            ViewBag.KeywordFilter = keywordFilter;
            ViewBag.Types = types;

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

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (User.IsInRole("promotor") && !string.IsNullOrEmpty(currentUserId))
            {
                var reservableTopicsCount = await GetReservableTopicsCountForPromotorAsync(currentUserId);
                ViewBag.ReservableTopicsCount = reservableTopicsCount;
                ViewBag.MaxReservableTopics = MaxReservableTopicsPerPromotor;
            }

            return View(topic);
        }

        [Authorize(Roles = "student,promotor")]
        public async Task<IActionResult> ExportTxt(int id)
        {
            var topic = await _context.Topics
                .Include(t => t.Promotor)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (topic == null)
            {
                return NotFound();
            }

            var content = BuildTxtContent(topic);
            var bytes = new UTF8Encoding(true).GetBytes(content);
            var fileName = BuildExportFileName(topic, "txt");

            return File(bytes, "text/plain; charset=utf-8", fileName);
        }

        [Authorize(Roles = "student,promotor")]
        public async Task<IActionResult> ExportCsv(int id)
        {
            var topic = await _context.Topics
                .Include(t => t.Promotor)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (topic == null)
            {
                return NotFound();
            }

            var content = BuildCsvContent(topic);
            var bytes = new UTF8Encoding(true).GetBytes(content);
            var fileName = BuildExportFileName(topic, "csv");

            return File(bytes, "text/csv; charset=utf-8", fileName);
        }

        [Authorize(Roles = "student,promotor")]
        public async Task<IActionResult> ExportPdf(int id)
        {
            var topic = await _context.Topics
                .Include(t => t.Promotor)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (topic == null)
            {
                return NotFound();
            }

            var bytes = BuildPdfContent(topic);
            var fileName = BuildExportFileName(topic, "pdf");

            return File(bytes, "application/pdf", fileName);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Type,Description,Keywords,CreatedDate,Status,PromotorId,KierownikId,DziekanId,StudentId,ReservationDate,ReservationStatus,RejectionReason,SubmittedDate,ApprovedDate")] Topic topic)
        {
            if (id != topic.Id)
            {
                return NotFound();
            }
            ModelState.Remove("Promotor");
            ModelState.Remove("Kierownik");
            ModelState.Remove("Dziekan");
            ModelState.Remove("Student");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(topic);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Zmiany zostały pomyślnie zapisane.";

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Topics.Any(e => e.Id == topic.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
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
            model.Status = TopicStatus.Draft;
            model.CreatedDate = DateTime.Now;
            ModelState.Remove("PromotorId");
            ModelState.Remove("Status");
            ModelState.Remove("Promotor");

            if (ModelState.IsValid)
            {
                _context.Topics.Add(model);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Nowy temat został utworzony jako szkic.";
                return RedirectToAction(nameof(Index));
            }
            return View(model);
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

            var reservableTopicsCount = await GetReservableTopicsCountForPromotorAsync(currentUserId!);
            if (reservableTopicsCount >= MaxReservableTopicsPerPromotor)
            {
                TempData["ErrorMessage"] = $"Osiągnięto limit {MaxReservableTopicsPerPromotor} tematów dostępnych do rezerwacji. Nie możesz wysłać kolejnego tematu do akceptacji.";
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

            var hasActiveTopic = await _context.Topics.AnyAsync(t => t.StudentId == currentUserId);

            if (hasActiveTopic)
            {
                TempData["ErrorMessage"] = "Masz już zarezerwowany lub przypisany temat. Anuluj oczekującą rezerwację, aby wybrać inny.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var topic = await _context.Topics
                .Include(t => t.Promotor)
                .FirstOrDefaultAsync(t => t.Id == id);

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
            topic.ReservationStatus = ReservationStatus.Pending;

            await _context.SaveChangesAsync();

            var student = await _context.Users.FindAsync(currentUserId);
            if (student != null && topic.Promotor != null)
            {
                await _notificationService.CreateTopicReservedNotificationAsync(topic, student);
            }

            TempData["SuccessMessage"] = "Temat został zarezerwowany pomyślnie.";

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "student")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var topic = await _context.Topics
                .Include(t => t.Promotor)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (topic == null)
                return NotFound();

            if (topic.StudentId != currentUserId)
            {
                TempData["ErrorMessage"] = "Nie możesz anulować rezerwacji, która nie należy do Ciebie.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var effective = topic.ReservationStatus ?? ReservationStatus.Accepted;
            if (effective != ReservationStatus.Pending)
            {
                TempData["ErrorMessage"] = "Możesz anulować tylko rezerwację oczekującą na promotora. Po akceptacji skontaktuj się z promotorem.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var student = await _context.Users.FindAsync(currentUserId);
            if (student != null && topic.Promotor != null)
            {
                await _notificationService.CreateReservationCancelledNotificationAsync(topic, student);
            }

            topic.StudentId = null;
            topic.ReservationDate = null;
            topic.ReservationStatus = null;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Rezerwacja została anulowana. Promotor otrzymał powiadomienie.";

            return RedirectToAction(nameof(Details), new { id });
        }

        private Task<int> GetReservableTopicsCountForPromotorAsync(string promotorId)
        {
            return _context.Topics.CountAsync(t =>
                t.PromotorId == promotorId &&
                t.Status == TopicStatus.Approved &&
                t.StudentId == null);
        }

        private static string BuildTxtContent(Topic topic)
        {
            var promotorFullName = GetPromotorFullName(topic);
            var description = string.IsNullOrWhiteSpace(topic.Description) ? "Brak opisu" : topic.Description;
            var keywords = string.IsNullOrWhiteSpace(topic.Keywords) ? "Brak" : topic.Keywords;

            var builder = new StringBuilder();
            builder.AppendLine($"Temat: {topic.Title}");
            builder.AppendLine($"Opis: {description}");
            builder.AppendLine($"Promotor: {promotorFullName}");
            builder.AppendLine($"Słowa kluczowe: {keywords}");
            builder.AppendLine($"Status: {topic.Status}");

            return builder.ToString();
        }

        private static string BuildCsvContent(Topic topic)
        {
            var promotorFullName = GetPromotorFullName(topic);
            var description = string.IsNullOrWhiteSpace(topic.Description) ? "Brak opisu" : topic.Description;
            var keywords = string.IsNullOrWhiteSpace(topic.Keywords) ? "Brak" : topic.Keywords;

            var builder = new StringBuilder();
            builder.AppendLine("Temat,Opis,Promotor,Słowa kluczowe,Status");
            builder.AppendLine(string.Join(",",
                EscapeCsv(topic.Title),
                EscapeCsv(description),
                EscapeCsv(promotorFullName),
                EscapeCsv(keywords),
                EscapeCsv(topic.Status.ToString())));

            return builder.ToString();
        }

        private static byte[] BuildPdfContent(Topic topic)
        {
            var promotorFullName = GetPromotorFullName(topic);
            var description = string.IsNullOrWhiteSpace(topic.Description) ? "Brak opisu" : topic.Description;
            var keywords = string.IsNullOrWhiteSpace(topic.Keywords) ? "Brak" : topic.Keywords;

            QuestPDF.Settings.License = LicenseType.Community;

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(style => style.FontFamily("Arial").FontSize(12));

                    page.Header()
                        .Text("Eksport tematu")
                        .FontSize(18)
                        .SemiBold();

                    page.Content().Column(column =>
                    {
                        column.Spacing(8);
                        column.Item().Text($"Temat: {topic.Title}");
                        column.Item().Text($"Opis: {description}");
                        column.Item().Text($"Promotor: {promotorFullName}");
                        column.Item().Text($"Słowa kluczowe: {keywords}");
                        column.Item().Text($"Status: {topic.Status}");
                    });
                });
            }).GeneratePdf();
        }

        private static string GetPromotorFullName(Topic topic)
        {
            if (topic.Promotor == null)
            {
                return "Brak danych";
            }

            return $"{topic.Promotor.FirstName} {topic.Promotor.LastName}".Trim();
        }

        private static string BuildExportFileName(Topic topic, string extension)
        {
            var safeTitle = SanitizeFileName(topic.Title);
            return $"{safeTitle}_{topic.Id}.{extension}";
        }

        private static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return "temat";
            }

            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = new string(fileName
                .Select(character => invalidChars.Contains(character) ? '_' : character)
                .ToArray())
                .Trim();

            return string.IsNullOrWhiteSpace(sanitized) ? "temat" : sanitized;
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
    }
}
