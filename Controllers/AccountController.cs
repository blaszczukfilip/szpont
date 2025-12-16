using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using szpont.Models;
using szpont.Services;

namespace szpont.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, szpont.Services.IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var indexExists = await _userManager.Users.AnyAsync(u => u.StudentIndex == vm.StudentIndex);
            if (indexExists)
            {
                ModelState.AddModelError(nameof(vm.StudentIndex), "This index has been already used");
                return View(vm);
            }

            var user = new ApplicationUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                StudentIndex = vm.StudentIndex
            };

            var result = await _userManager.CreateAsync(user, vm.Password);
            if (result.Succeeded)
            {
                // ustawienie domyslnie roli student
                await _userManager.AddToRoleAsync(user, "student");

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(vm);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View(model);
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                var lockoutEnd = user.LockoutEnd ?? DateTimeOffset.UtcNow;
                ModelState.AddModelError("", $"Konto jest zablokowane do {lockoutEnd.ToLocalTime():dd.MM.yyyy HH:mm}");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("admin"))
                    return RedirectToAction("Index", "AdminDashboard");
                if (roles.Contains("dziekan"))
                    return RedirectToAction("Index", "DziekanDashboard");
                if (roles.Contains("kierownik"))
                    return RedirectToAction("Index", "KierownikDashboard");
                if (roles.Contains("promotor"))
                    return RedirectToAction("Index", "PromotorDashboard");
                if (roles.Contains("student"))
                    return RedirectToAction("Index", "StudentDashboard");

                // domyślne przekierowanie
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid email or password");
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgetPassword model) {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("ForgotPasswordConfirmation");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetUrl = Url.Action(
                "ResetPassword",
                "Account",
                new { email = user.Email, token},
                protocol: HttpContext.Request.Scheme);

            await _emailSender.SendEmailAsync(
                user.Email,
                "Reset hasła",
                $"Kliknij <a href=\"{resetUrl}\">tutaj</a>, aby zresetować hasło.");

                return RedirectToAction("ForgotPasswordConfirmation");
        }
         [AllowAnonymous]
         public IActionResult ForgotPasswordConfirmation()
         {
            return View();
         }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (email == null || token == null)
            {
                return RedirectToAction("Login");
            }
            var model = new ResetPassword
            {
                Email = email,
                Token = token
            };
            return View(model);
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPassword model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction("ResetPasswordConfirmation");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
                return RedirectToAction("ResetPasswordConfirmation");

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }



    }
}
