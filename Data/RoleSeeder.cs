using Microsoft.AspNetCore.Identity;
using szpont.Models;

namespace szpont.Data
{
    public static class RoleSeeder
    {
        private static readonly string[] DefaultRoles = new[]
        {
            "student",
            "dziekan",
            "kierownik",
            "promotor",
            "admin"
        };

        private const string AdminEmail = "admin@szpont.local";
        private const string PromotorEmail = "promotor@szpont.local";
        private const string StudentEmail = "student@szpont.local";
        private const string AdminPassword = "Admin123!";
        private const string PromotorPassword = "Promotor123!";
        private const string StudentPassword = "Student123!";
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await EnsureRolesAsync(roleManager);
            await EnsureAdminAsync(userManager);
            await EnsurePromotorAsync(userManager);
            await EnsureStudentAsync(userManager);
        }


        // Stworzenie roli jesli nie istnieje
        private static async Task EnsureRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var roleName in DefaultRoles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        // Stworzenie admina jesli nie istnieje i nadanie mu roli admin
        private static async Task EnsureAdminAsync(UserManager<ApplicationUser> userManager)
        {
            var adminUser = await userManager.FindByEmailAsync(AdminEmail);

            if (adminUser is null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = AdminEmail,
                    Email = AdminEmail,
                    EmailConfirmed = true,
                    FirstName = "Administrator",
                    LastName = "System",
                    StudentIndex = "ADMIN0000"
                };

                var createResult = await userManager.CreateAsync(adminUser, AdminPassword);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Unable to create default admin user: {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(adminUser, "admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "admin");
            }
        }

        private static async Task EnsurePromotorAsync(UserManager<ApplicationUser> userManager)
        {
            var promotorUser = await userManager.FindByEmailAsync(PromotorEmail);

            if (promotorUser is null)
            {
                promotorUser = new ApplicationUser
                {
                    UserName = PromotorEmail,
                    Email = PromotorEmail,
                    EmailConfirmed = true,
                    FirstName = "Piotr",
                    LastName = "Nowak",
                    StudentIndex = "PROM00000"
                };

                var createResult = await userManager.CreateAsync(promotorUser, PromotorPassword);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Unable to create default promotor user: {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(promotorUser, "promotor"))
            {
                await userManager.AddToRoleAsync(promotorUser, "promotor");
            }
        }

        private static async Task EnsureStudentAsync(UserManager<ApplicationUser> userManager)
        {
            var studentUser = await userManager.FindByEmailAsync(StudentEmail);

            if (studentUser is null)
            {
                studentUser = new ApplicationUser
                {
                    UserName = StudentEmail,
                    Email = StudentEmail,
                    EmailConfirmed = true,
                    FirstName = "Anna",
                    LastName = "Wojcik",
                    StudentIndex = "STUDENT00"
                };

                var createResult = await userManager.CreateAsync(studentUser, StudentPassword);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Unable to create default student user: {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(studentUser, "student"))
            {
                await userManager.AddToRoleAsync(studentUser, "student");
            }
        }
    }
}
