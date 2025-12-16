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
                    },
                    new Topic
                    {
                        Title = "Aplikacja webowa do zarządzania projektami zespołowymi",
                        Description = "Opracowanie systemu kanban z obsługą wielu użytkowników, komentarzy oraz integracją z zewnętrznymi API.",
                        Type = "Inżynierska",
                        Keywords = "aplikacja webowa, zarządzanie projektami, kanban, ASP.NET Core"
                    },
                    new Topic
                    {
                        Title = "Platforma e‑learningowa z modułem testów adaptacyjnych",
                        Description = "Stworzenie platformy do kursów online z możliwością generowania testów o różnym poziomie trudności na podstawie wyników użytkownika.",
                        Type = "Inżynierska",
                        Keywords = "e-learning, testy, adaptacyjne, edukacja"
                    },
                    new Topic
                    {
                        Title = "Analiza bezpieczeństwa aplikacji webowych w architekturze mikroserwisowej",
                        Description = "Identyfikacja typowych podatności oraz zaprojektowanie mechanizmów ochrony komunikacji pomiędzy mikroserwisami.",
                        Type = "Magisterska",
                        Keywords = "bezpieczeństwo, mikroserwisy, OWASP, JWT"
                    },
                    new Topic
                    {
                        Title = "System monitorowania jakości powietrza z wykorzystaniem IoT",
                        Description = "Budowa prototypu czujników oraz aplikacji zbierającej i wizualizującej dane o zanieczyszczeniu powietrza w czasie rzeczywistym.",
                        Type = "Inżynierska",
                        Keywords = "IoT, sensor, jakość powietrza, wizualizacja danych"
                    },
                    new Topic
                    {
                        Title = "Wykorzystanie sieci neuronowych do klasyfikacji obrazu medycznego",
                        Description = "Badanie skuteczności CNN w klasyfikacji obrazów RTG oraz przygotowanie pipeline'u przetwarzania danych.",
                        Type = "Magisterska",
                        Keywords = "deep learning, CNN, medycyna, klasyfikacja obrazów"
                    },
                    new Topic
                    {
                        Title = "Porównanie wydajności baz danych relacyjnych i nierelacyjnych w aplikacjach webowych",
                        Description = "Przeprowadzenie eksperymentów wydajnościowych dla popularnych silników baz danych w scenariuszach CRUD.",
                        Type = "Inżynierska",
                        Keywords = "bazy danych, SQL, NoSQL, wydajność"
                    },
                    new Topic
                    {
                        Title = "Aplikacja do zarządzania budżetem domowym z analizą wydatków",
                        Description = "Implementacja systemu kategoryzacji transakcji i generowania raportów finansowych z wykorzystaniem wykresów.",
                        Type = "Inżynierska",
                        Keywords = "finanse osobiste, budżet, raporty, wykresy"
                    },
                    new Topic
                    {
                        Title = "System obsługi konferencji naukowych",
                        Description = "Projekt systemu do rejestracji uczestników, zgłaszania referatów oraz generowania harmonogramu wystąpień.",
                        Type = "Inżynierska",
                        Keywords = "konferencja, rejestracja, harmonogram, system rezerwacji"
                    },
                    new Topic
                    {
                        Title = "Analiza wykorzystania konteneryzacji w procesie CI/CD",
                        Description = "Projekt i implementacja pipeline'u CI/CD z użyciem Dockera i systemu orkiestracji kontenerów.",
                        Type = "Magisterska",
                        Keywords = "CI/CD, Docker, DevOps, kontenery"
                    },
                    new Topic
                    {
                        Title = "Portal ogłoszeniowy z mechanizmem wyszukiwania pełnotekstowego",
                        Description = "Stworzenie serwisu ogłoszeniowego z zaawansowaną wyszukiwarką oraz filtrowaniem wyników.",
                        Type = "Inżynierska",
                        Keywords = "wyszukiwanie, full-text search, ogłoszenia, filtracja"
                    },
                    new Topic
                    {
                        Title = "Aplikacja wspierająca naukę programowania dla początkujących",
                        Description = "Opracowanie platformy z zadaniami programistycznymi, automatyczną oceną rozwiązań i podpowiedziami.",
                        Type = "Inżynierska",
                        Keywords = "nauka programowania, edukacja, ocena automatyczna"
                    },
                    new Topic
                    {
                        Title = "System zarządzania dokumentacją techniczną w przedsiębiorstwie",
                        Description = "Projekt repozytorium dokumentów z kontrolą wersji, uprawnieniami dostępu oraz historią zmian.",
                        Type = "Magisterska",
                        Keywords = "dokumentacja, wersjonowanie, uprawnienia, workflow"
                    }
                });
                await context.SaveChangesAsync();
            }
        }
    }
}
