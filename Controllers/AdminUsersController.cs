using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using szpont.Data;
using szpont.Models;
using System.ComponentModel.DataAnnotations;


namespace szpont.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminUsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminUsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index(
            string searchTerm,
            string roleFilter,
            int page = 1,
            //na pojedynczej stronie 10 użytkowników
            int pageSize = 10)
        {
            var users = _userManager.Users.AsQueryable();

            //filotrwanie po tekscie - string (imię, nazwisko, email, indeks)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                users = users.Where(u =>
                    u.FirstName.Contains(searchTerm) ||
                    u.LastName.Contains(searchTerm) ||
                    u.Email.Contains(searchTerm) ||
                    u.StudentIndex.Contains(searchTerm));
            }

            //filtrowanie po roli
            if (!string.IsNullOrWhiteSpace(roleFilter))
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(roleFilter);
                var userIdsInRole = usersInRole.Select(u => u.Id).ToList();
                users = users.Where(u => userIdsInRole.Contains(u.Id));
            }

            //pobranie ról
            var roles = await _roleManager.Roles.Select(r => r.Name!).OrderBy(r => r).ToListAsync();

            //liczba użytkowników (potrzebne do paginacji)
            var totalCount = await users.CountAsync();

            //paginacja
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            page = Math.Max(1, Math.Min(page, totalPages == 0 ? 1 : totalPages));

            var usersList = await users
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            //pobranie ról dla każdego użytkownika (ViewModel)
            var usersWithRoles = new List<UserWithRolesViewModel>();
            foreach (var user in usersList)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                usersWithRoles.Add(new UserWithRolesViewModel
                {
                    User = user,
                    Roles = userRoles.ToList()
                });
            }

            //dane przekazywane do widoku
            ViewBag.SearchTerm = searchTerm;
            ViewBag.RoleFilter = roleFilter;
            ViewBag.Roles = roles;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;
            ViewBag.PageSize = pageSize;

            return View(usersWithRoles);
        }

        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.Select(r => r.Name!).OrderBy(r => r).ToListAsync();

            var viewModel = new UserDetailsViewModel
            {
                User = user,
                Roles = userRoles.ToList(),
                AllRoles = allRoles
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var allRoles = await _roleManager.Roles
                .Select(r => r.Name!)
                .OrderBy(r => r)
                .ToListAsync();

            var userRoles = await _userManager.GetRolesAsync(user);

            var viewModel = new EditUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Role = userRoles.FirstOrDefault() ?? string.Empty
            };

            ViewBag.Roles = allRoles;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            var allRoles = await _roleManager.Roles
                .Select(r => r.Name!)
                .OrderBy(r => r)
                .ToListAsync();

            ViewBag.Roles = allRoles;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!model.Email.Contains('.') || model.Email.EndsWith("."))
            {
                ModelState.AddModelError(nameof(model.Email), "Email musi zawierać domenę, np. .pl lub .com");
                return View(model);
            }


            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(model.Role) || !allRoles.Contains(model.Role))
            {
                ModelState.AddModelError(nameof(model.Role), "Wybrana rola nie istnieje.");
                return View(model);
            }
            // aktualizacja danych
            user.FirstName = model.FirstName.Trim();
            user.LastName = model.LastName.Trim();
            user.Email = model.Email.Trim();
            user.UserName = model.Email.Trim();

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            // zmiana roli
            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    foreach (var error in removeResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            var addResult = await _userManager.AddToRoleAsync(user, model.Role);
            if (!addResult.Succeeded)
            {
                foreach (var error in addResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            TempData["SuccessMessage"] = "Zapisano zmiany użytkownika.";
            return RedirectToAction("Details", new { id = user.Id });
        }

        // blokowanie
        public async Task<IActionResult> Block(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // potwierdzenie blokady
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlockConfirmed(string id, int lockoutHours)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            if (!user.LockoutEnabled)
                user.LockoutEnabled = true;

            var lockoutEnd = DateTimeOffset.UtcNow.AddHours(lockoutHours);
            await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);

            return RedirectToAction(nameof(Details), new { id });
        }


        // odblokowanie
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unblock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            await _userManager.SetLockoutEndDateAsync(user, null);

            return RedirectToAction(nameof(Details), new { id });
        }


    }

    //ViewModel dla listy użytkowników z rolami - wykorzystane w Index
    public class UserWithRolesViewModel
    {
        public ApplicationUser User { get; set; } = null!;
        public List<string> Roles { get; set; } = new();
    }

    //ViewModel dla szczegółów użytkownika - wykorzystane w Details
    public class UserDetailsViewModel
    {
        public ApplicationUser User { get; set; } = null!;
        public List<string> Roles { get; set; } = new();
        public List<string> AllRoles { get; set; } = new();
    }

    // ViewModel dla edycji uzytkownika
    public class EditUserViewModel
    {
        [Required(ErrorMessage = "Imię jest wymagane")]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Niepoprawny format email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rola jest wymagana")]
        public string Role { get; set; } = string.Empty;

        [Required]
        public string Id { get; set; } = string.Empty;
    }

}

