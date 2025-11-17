## 📄 README.md - System Zarządzania Tematami Prac Dyplomowych (SZPONT)

## 🚀 Wprowadzenie

Aplikacja SZPONT (System Zarządzania Tematami Prac Dyplomowych) to centralna platforma webowa służąca do kompleksowego zarządzania cyklem życia tematów prac dyplomowych (inżynierskich i magisterskich) na uczelni. System automatyzuje i usprawnia procesy: od zgłoszenia tematu przez Promotora, poprzez formalną akceptację przez władze wydziału, aż po wybór tematu przez Studenta.

## 🛠️ Architektura i Technologie

| Komponent | Technologia / Wersja | Opis |
| :--- | :--- | :--- |
| **Backend** | **ASP.NET Core 8.0 (MVC)** | Implementacja logiki biznesowej, API i Warstwy Dostępu do Danych dla aplikacji SZPONT. |
| **Frontend** | Razor Pages / HTML / CSS / JavaScript | Warstwa prezentacji, zapewniająca interfejsy użytkownika dostosowane do ról. |
| **Baza Danych** | **Entity Framework Core 8** (z Providerem np. **PostgreSQL / SQL Server**) | ORM do komunikacji z bazą danych przechowującą dane tematów. |
| **Kontrola Wersji** | Git (Repozytorium: `https://devtools.wi.pb.edu.pl/bitbucket/scm/szpont/szpont.git`) | Zarządzanie kodem źródłowym projektu SZPONT. |

## 🔑 Kluczowe Funkcjonalności

* **Zarządzanie Rolami:** Obsługa ról: **Administrator, Dziekan, Kierownik Katedry, Promotor, Student**.
* **Workflow Akceptacji:** Trzyetapowa, formalna ścieżka akceptacji tematów: **Promotor --> Kierownik Katedry --> Dziekan**.
* **Rezerwacja Tematów:** Interfejs dla studentów do przeglądania, filtrowania, wyszukiwania i rezerwacji dostępnych tematów.
* **Integracja z USOS:** Moduł eksportu zaakceptowanych tematów i przypisanych studentów, kluczowy dla formalnego procedowania prac.
* **Powiadomienia:** Wbudowany system powiadomień i e-mail (np. o zmianie statusu tematu lub konieczności akceptacji).
* **Limity i Terminy:** Monitorowanie i definiowanie limitów prac dla promotorów oraz katedr.

## ⚙️ Wymagania i Uruchomienie

### Wymagania Środowiskowe
* **.NET 8 SDK**
* **Git**
* Serwer bazodanowy (**PostgreSQL** lub **SQL Server**)

### Instrukcja Uruchomienia SZPONT
1.  **Klonowanie Repozytorium:**
    ```bash
    git clone [https://devtools.wi.pb.edu.pl/bitbucket/scm/szpont/szpont.git](https://devtools.wi.pb.edu.pl/bitbucket/scm/szpont/szpont.git)
    cd szpont
    ```
2.  **Konfiguracja Bazy Danych:**
    * Wykonaj migracje Entity Framework Core, aby utworzyć schemat bazy danych:
        ```bash
        dotnet ef database update
        ```
3.  **Uruchomienie Aplikacji:**
    ```bash
    dotnet run
    ```
    Aplikacja **SZPONT** będzie dostępna pod adresem wskazanym w konsoli.



---

