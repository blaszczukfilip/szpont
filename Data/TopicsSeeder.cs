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
                context.Topics.AddRange(new[]
                {
                    new Topic
                    {
                        Title = "Analiza efektywności algorytmów uczenia maszynowego",
                        Description = "Porównanie wybranych algorytmów ML pod kątem wydajności, jakości predykcji oraz zastosowania w rzeczywistych projektach.",
                        Type = "Inżynierska",
                        Keywords = "machine learning, AI, algorytmy, analiza"
                    },
                    new Topic
                    {
                        Title = "Projekt i implementacja aplikacji mobilnej wspierającej zdrowe nawyki",
                        Description = "Stworzenie aplikacji z funkcjami monitorowania aktywności, przypomnieniami oraz analizą danych użytkownika.",
                        Type = "Inżynierska",
                        Keywords = "mobile app, zdrowie, Android, iOS"
                    },
                    new Topic
                    {
                        Title = "System rekomendacji produktów na podstawie analizy zachowań użytkowników",
                        Description = "Badanie technik filtrowania kolaboratywnego i content-based oraz stworzenie prototypu systemu rekomendacyjnego.",
                        Type = "Magisterska",
                        Keywords = "rekomendacje, analiza danych, AI, użytkownicy"
                    }
                });
                await context.SaveChangesAsync();
            }
        }
    }
}
