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
        private const string AdminPassword = "Admin123!";
public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await EnsureRolesAsync(roleManager);
            await EnsureAdminAsync(userManager);
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
    }
}
