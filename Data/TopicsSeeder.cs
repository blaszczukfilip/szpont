using Microsoft.Extensions.DependencyInjection;
using szpont.Models;

namespace szpont.Data
{
    public static class TopicSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();

            if (!context.Topics.Any())
            {
                context.Topics.Add(new Topic
                {
                    Title = "Pierwszy temat",
                    Description = "Opis testowy",
                    Type = "General",
                    Keywords = "test,example",
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
