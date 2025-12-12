using Microsoft.AspNetCore.Identity;
using szpont.Models;

namespace szpont.Data
{
    public static class UsersSeeder
    {
        private const string DefaultPassword = "User123!";

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Sprawdzenie czy użytkownicy już istnieją (sprawdzamy czy istnieje UserA)
            var existingUser = await userManager.FindByEmailAsync("usera@email.com");
            if (existingUser != null)
            {
                return; // Seeder już został wykonany
            }

            // Utworzenie 25 użytkowników
            var users = new List<(ApplicationUser user, string role)>();

            // UserA-UserO (15 studentów) - indeksy 100001-100015
            for (int i = 0; i < 15; i++)
            {
                char letter = (char)('A' + i);
                int index = 100001 + i;
                var user = new ApplicationUser
                {
                    UserName = $"user{char.ToLower(letter)}@email.com",
                    Email = $"user{char.ToLower(letter)}@email.com",
                    EmailConfirmed = true,
                    FirstName = $"User{letter}",
                    LastName = letter.ToString(),
                    StudentIndex = index.ToString()
                };
                users.Add((user, "student"));
            }

            // UserP-UserT (5 promotorów) - indeksy 100016-100020
            for (int i = 0; i < 5; i++)
            {
                char letter = (char)('P' + i);
                int index = 100016 + i;
                var user = new ApplicationUser
                {
                    UserName = $"user{char.ToLower(letter)}@email.com",
                    Email = $"user{char.ToLower(letter)}@email.com",
                    EmailConfirmed = true,
                    FirstName = $"User{letter}",
                    LastName = letter.ToString(),
                    StudentIndex = index.ToString()
                };
                users.Add((user, "promotor"));
            }

            // UserU-UserW (3 kierowników) - indeksy 100021-100023
            for (int i = 0; i < 3; i++)
            {
                char letter = (char)('U' + i);
                int index = 100021 + i;
                var user = new ApplicationUser
                {
                    UserName = $"user{char.ToLower(letter)}@email.com",
                    Email = $"user{char.ToLower(letter)}@email.com",
                    EmailConfirmed = true,
                    FirstName = $"User{letter}",
                    LastName = letter.ToString(),
                    StudentIndex = index.ToString()
                };
                users.Add((user, "kierownik"));
            }

            // UserX-UserY (2 dziekanów) - indeksy 100024-100025
            for (int i = 0; i < 2; i++)
            {
                char letter = (char)('X' + i);
                int index = 100024 + i;
                var user = new ApplicationUser
                {
                    UserName = $"user{char.ToLower(letter)}@email.com",
                    Email = $"user{char.ToLower(letter)}@email.com",
                    EmailConfirmed = true,
                    FirstName = $"User{letter}",
                    LastName = letter.ToString(),
                    StudentIndex = index.ToString()
                };
                users.Add((user, "dziekan"));
            }

            // Utworzenie użytkowników i przypisanie ról
            foreach (var (user, role) in users)
            {
                var createResult = await userManager.CreateAsync(user, DefaultPassword);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Unable to create user {user.Email}: {errors}");
                }

                if (!await userManager.IsInRoleAsync(user, role))
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}

