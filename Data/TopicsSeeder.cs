using Microsoft.Extensions.DependencyInjection;
using szpont.Models;
using Microsoft.AspNetCore.Identity;

namespace szpont.Data
{
    public static class TopicSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            var promotorzy = await userManager.GetUsersInRoleAsync(RoleNames.Promotor);
            var kierownicy = await userManager.GetUsersInRoleAsync(RoleNames.Kierownik);
            var dziekani = await userManager.GetUsersInRoleAsync(RoleNames.Dziekan);

            string pId = promotorzy.FirstOrDefault()?.Id;
            string? kId = kierownicy.FirstOrDefault()?.Id;
            string? dId = dziekani.FirstOrDefault()?.Id;

            if (!context.Topics.Any())
            {
                context.Topics.AddRange(new[]
                {
                    new Topic
                    {
                        Title = "Analiza efektywności algorytmów uczenia maszynowego",
                        Description = "Porównanie wybranych algorytmów ML pod kątem wydajności, jakości predykcji oraz zastosowania w rzeczywistych projektach.",
                        Type = "Inżynierska",
                        Keywords = "machine learning, AI, algorytmy, analiza",
                        Status = TopicStatus.Approved,
                        PromotorId = pId,
                        KierownikId = kId,
                        ApprovedDate = DateTime.Now.AddDays(-10)
                    },
                    new Topic
                    {
                        Title = "Projekt i implementacja aplikacji mobilnej wspierającej zdrowe nawyki",
                        Description = "Stworzenie aplikacji z funkcjami monitorowania aktywności, przypomnieniami oraz analizą danych użytkownika.",
                        Type = "Inżynierska",
                        Keywords = "mobile app, zdrowie, Android, iOS",
                        Status = TopicStatus.WaitingForKierownik,
                        PromotorId = pId,
                        SubmittedDate = DateTime.Now.AddDays(-2)
                    },  
                    new Topic
                    { 
                        Title = "System rekomendacji produktów na podstawie analizy zachowań użytkowników",
                        Description = "Badanie technik filtrowania kolaboratywnego i content-based oraz stworzenie prototypu systemu rekomendacyjnego.",
                        Type = "Magisterska",
                        Keywords = "rekomendacje, analiza danych, AI, użytkownicy",
                        Status = TopicStatus.Draft,
                        PromotorId = pId
                    },
                    new Topic 
                    { 
                        Title = "Aplikacja webowa do zarządzania projektami zespołowymi", 
                        Description = "Opracowanie systemu kanban z obsługą wielu użytkowników, komentarzy oraz integracją z zewnętrznymi API.",
                        Type = "Inżynierska", 
                        Keywords = "aplikacja webowa, zarządzanie projektami, kanban, ASP.NET Core",
                        Status = TopicStatus.WaitingForDziekan, 
                        PromotorId = pId, 
                        KierownikId = kId, 
                        SubmittedDate = DateTime.Now.AddDays(-1) 
                    },
                    new Topic 
                    { 
                        Title = "Platforma e‑learningowa z modułem testów adaptacyjnych",
                        Description = "Stworzenie platformy do kursów online z możliwością generowania testów o różnym poziomie trudności na podstawie wyników użytkownika.",
                        Type = "Inżynierska", 
                        Keywords = "e-learning, edukacja", 
                        Status = TopicStatus.Draft, 
                        PromotorId = pId 
                    },
                    new Topic 
                    { 
                        Title = "Analiza bezpieczeństwa aplikacji webowych w architekturze mikroserwisowej", 
                        Description = "Identyfikacja typowych podatności oraz zaprojektowanie mechanizmów ochrony komunikacji pomiędzy mikroserwisami.",
                        Type = "Magisterska",
                        Keywords = "bezpieczeństwo, mikroserwisy, OWASP, JWT",
                        Status = TopicStatus.Rejected, 
                        PromotorId = pId, 
                        RejectionReason = "Temat zbyt ogólny.", 
                        CreatedDate = DateTime.Now.AddDays(-5) 
                    },
                    new Topic 
                    { 
                       Title = "System monitorowania jakości powietrza z wykorzystaniem IoT",
                        Description = "Budowa prototypu czujników oraz aplikacji zbierającej i wizualizującej dane o zanieczyszczeniu powietrza w czasie rzeczywistym.",
                        Type = "Inżynierska",
                        Keywords = "IoT, sensor, jakość powietrza, wizualizacja danych",
                        Status = TopicStatus.Approved, 
                        PromotorId = pId, 
                        KierownikId = kId, 
                        DziekanId = dId, 
                        ApprovedDate = DateTime.Now.AddDays(-1) 
                    },
                    new Topic 
                    { 
                        Title = "Wykorzystanie sieci neuronowych do klasyfikacji obrazu medycznego",
                        Description = "Badanie skuteczności CNN w klasyfikacji obrazów RTG oraz przygotowanie pipeline'u przetwarzania danych.",
                        Type = "Magisterska",
                        Keywords = "deep learning, CNN, medycyna, klasyfikacja obrazów",
                        Status = TopicStatus.WaitingForKierownik, 
                        PromotorId = pId, 
                        SubmittedDate = DateTime.Now.AddHours(-12) 
                    },
                    new Topic 
                    { 
                        Title = "Porównanie wydajności baz danych relacyjnych i nierelacyjnych w aplikacjach webowych",
                        Description = "Przeprowadzenie eksperymentów wydajnościowych dla popularnych silników baz danych w scenariuszach CRUD.",
                        Type = "Inżynierska",
                        Keywords = "bazy danych, SQL, NoSQL, wydajność",
                        Status = TopicStatus.Draft, 
                        PromotorId = pId 
                    },
                    new Topic 
                    { 
                        Title = "Aplikacja do zarządzania budżetem domowym z analizą wydatków",
                        Description = "Implementacja systemu kategoryzacji transakcji i generowania raportów finansowych z wykorzystaniem wykresów.",
                        Type = "Inżynierska",
                        Keywords = "finanse osobiste, budżet, raporty, wykresy",
                        Status = TopicStatus.Approved, 
                        PromotorId = pId, 
                        KierownikId = kId, 
                        ApprovedDate = DateTime.Now.AddMonths(-1) 
                    },
                    new Topic 
                    { 
                        Title = "System obsługi konferencji naukowych",
                        Description = "Projekt systemu do rejestracji uczestników, zgłaszania referatów oraz generowania harmonogramu wystąpień.",
                        Type = "Inżynierska",
                        Keywords = "konferencja, rejestracja, harmonogram, system rezerwacji",
                        Status = TopicStatus.Draft, 
                        PromotorId = pId
                    },
                    new Topic 
                    { 
                        Title = "Analiza wykorzystania konteneryzacji w procesie CI/CD",
                        Description = "Projekt i implementacja pipeline'u CI/CD z użyciem Dockera i systemu orkiestracji kontenerów.",
                        Type = "Magisterska",
                        Keywords = "CI/CD, Docker, DevOps, kontenery",
                        Status = TopicStatus.WaitingForKierownik, 
                        PromotorId = pId, 
                        SubmittedDate = DateTime.Now.AddDays(-3) 
                    },
                    new Topic 
                    { 
                        Title = "Portal ogłoszeniowy z mechanizmem wyszukiwania pełnotekstowego",
                        Description = "Stworzenie serwisu ogłoszeniowego z zaawansowaną wyszukiwarką oraz filtrowaniem wyników.",
                        Type = "Inżynierska",
                        Keywords = "wyszukiwanie, full-text search, ogłoszenia, filtracja",
                        Status = TopicStatus.Draft, 
                        PromotorId = pId 
                    },
                    new Topic 
                    { 
                        Title = "Aplikacja wspierająca naukę programowania dla początkujących",
                        Description = "Opracowanie platformy z zadaniami programistycznymi, automatyczną oceną rozwiązań i podpowiedziami.",
                        Type = "Inżynierska",
                        Keywords = "nauka programowania, edukacja, ocena automatyczna",
                        Status = TopicStatus.Approved, 
                        PromotorId = pId, 
                        KierownikId = kId, 
                        ApprovedDate = DateTime.Now 
                    },
                    new Topic 
                    { 
                        Title = "System zarządzania dokumentacją techniczną w przedsiębiorstwie",
                        Description = "Projekt repozytorium dokumentów z kontrolą wersji, uprawnieniami dostępu oraz historią zmian.",
                        Type = "Magisterska",
                        Keywords = "dokumentacja, wersjonowanie, uprawnienia, workflow",
                        Status = TopicStatus.WaitingForKierownik, 
                        PromotorId = pId, 
                        SubmittedDate = DateTime.Now.AddHours(-2) 
                    }
                });
                await context.SaveChangesAsync();
            }
        }
    }
}
