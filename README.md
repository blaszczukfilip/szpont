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

### Konto administratora
* E-mail: admin@szpont.local
* Hasło: Admin123!

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

## 📚 CRUD Tematów Prac Dyplomowych (Topics)

### Model `Topic`

Temat pracy dyplomowej reprezentuje encja `Topic`:

- **Id** – klucz główny (int)
- **Title** – tytuł tematu (wymagany, max 200 znaków)
- **Description** – opis tematu (opcjonalny, max 1000 znaków)
- **Type** – typ pracy (np. „Inżynierska”, „Magisterska”; wymagany, max 50 znaków)
- **Keywords** – słowa kluczowe rozdzielone przecinkami (max 200 znaków)
- **CreatedDate** – data utworzenia rekordu (ustawiana automatycznie)

### Dostęp do funkcjonalności (kontroler `TopicsController`)

Funkcje CRUD dla tematów są obsługiwane przez kontroler MVC `TopicsController`:

- **Lista tematów (Read / Index)**  
  - URL: `GET /Topics`  
  - Opis: Wyświetla listę wszystkich tematów z możliwością przejścia do szczegółów, edycji lub usunięcia (w zależności od uprawnień).

- **Szczegóły tematu (Read / Details)**  
  - URL: `GET /Topics/Details/{id}`  
  - Opis: Wyświetla szczegółowe informacje o wybranym temacie (`Title`, `Description`, `Type`, `Keywords`, `CreatedDate`).  
  - Zastosowanie: Używane zarówno przez promotorów, jak i studentów do analizy treści tematu.

- **Utworzenie nowego tematu (Create)**  
  - URL (formularz): `GET /Topics/Create`  
  - URL (zapis): `POST /Topics/Create`  
  - Opis: Formularz umożliwia wprowadzenie tytułu, opisu, typu pracy i słów kluczowych. Po poprawnej walidacji dane są zapisywane do bazy (`ApplicationDbContext.Topics`).  
  - Uprawnienia: Domyślnie przeznaczone dla ról typu **Promotor** / **Administrator** (zgodnie z konfiguracją autoryzacji w projekcie).

- **Edycja istniejącego tematu (Update)**  
  - URL (formularz): `GET /Topics/Edit/{id}`  
  - URL (zapis): `POST /Topics/Edit/{id}`  
  - Opis: Formularz wczytuje bieżące dane tematu i pozwala na ich modyfikację. Po zapisie EF Core aktualizuje rekord w tabeli `Topics`.  
  - Walidacja: Zachowane są ograniczenia modelu (`[Required]`, `[MaxLength]`).

- **Usunięcie tematu (Delete)**  
  - URL (potwierdzenie): `GET /Topics/Delete/{id}`  
  - URL (zapis): `POST /Topics/DeleteConfirmed/{id}` (domyślny wzorzec w ASP.NET Core MVC)  
  - Opis: Najpierw wyświetlane jest okno potwierdzenia, następnie temat jest usuwany z bazy danych.

### Seed danych tematów (stan początkowy)

Przy pierwszym uruchomieniu aplikacji (lub gdy tabela `Topics` jest pusta) system automatycznie zasila bazę przykładowymi tematami:

- Implementacja znajduje się w klasie `TopicSeeder` (`Data/TopicsSeeder.cs`).
- Wywołanie seeding-u odbywa się w `Program.cs` podczas startu aplikacji:
  - `await TopicSeeder.SeedAsync(services);`
- Seed zawiera **min. 15 zróżnicowanych tematów** (różne typy – inżynierskie/magisterskie, różne dziedziny, bogate słowa kluczowe), co pozwala od razu testować widoki listy, szczegółów, edycji i usuwania tematów.

## Umiejętności ról

W systemie SZPONT zastosowano kilka ról użytkowników. Każda rola ma przypisany osobny zakres uprawnień, dzięki czemu użytkownicy widzą tylko te funkcje, które są im potrzebne do pracy w systemie.

### Administrator

Administrator posiada najszersze uprawnienia w systemie. Ma dostęp do panelu administracyjnego, w którym może przeglądać statystyki dotyczące użytkowników, ról oraz tematów prac dyplomowych. Administrator może również wygenerować raport systemowy w formacie TXT lub CSV.

Administrator zarządza użytkownikami systemu. Może przeglądać listę kont, wyszukiwać użytkowników po imieniu, nazwisku, adresie e-mail lub numerze indeksu, filtrować użytkowników według roli oraz korzystać z paginacji listy. Ma także możliwość edycji danych użytkownika, zmiany jego roli, blokowania i odblokowywania kont.

Administrator może również zarządzać tematami prac dyplomowych. Ma możliwość tworzenia, edycji oraz usuwania tematów. W przeciwieństwie do promotora nie jest ograniczony tylko do tematów przypisanych do siebie.

### Student

Student może przeglądać listę dostępnych tematów prac dyplomowych. W widoku tematów student widzi tylko tematy zatwierdzone, które nie zostały jeszcze przypisane do żadnego innego studenta.

Student może zarezerwować wybrany temat. System sprawdza przy tym, czy student nie ma już aktywnej rezerwacji lub przypisanego tematu. Po rezerwacji temat otrzymuje status rezerwacji oczekującej na decyzję promotora.

Student może anulować swoją rezerwację, ale tylko wtedy, gdy jest ona jeszcze w stanie oczekiwania na akceptację promotora. Po zaakceptowaniu rezerwacji przez promotora anulowanie z poziomu systemu nie jest już dostępne.

Student ma również dostęp do swojego panelu, w którym widzi informacje o własnej rezerwacji, liczbę dostępnych tematów oraz powiadomienia dotyczące zmian w systemie.

### Promotor

Promotor może tworzyć nowe tematy prac dyplomowych. Nowo utworzony temat trafia najpierw do statusu szkicu, dzięki czemu promotor może go przygotować przed wysłaniem do akceptacji.

Promotor może edytować i usuwać tylko te tematy, których jest właścicielem. System blokuje możliwość modyfikacji tematu, jeżeli rezerwacja została już zaakceptowana.

Promotor może wysłać temat do akceptacji przez kierownika. Po wysłaniu temat zmienia status ze szkicu na oczekujący na decyzję kierownika. System kontroluje również limit tematów dostępnych do rezerwacji dla jednego promotora.

Promotor obsługuje rezerwacje tematów zgłaszane przez studentów. Może zaakceptować albo odrzucić rezerwację. Po zaakceptowaniu student zostaje przypisany do tematu, a po odrzuceniu temat wraca do puli dostępnych tematów.

W panelu promotora wyświetlane są statystyki jego tematów, między innymi liczba szkiców, tematów oczekujących na akceptację, tematów zatwierdzonych, odrzuconych oraz liczba oczekujących rezerwacji.

### Kierownik

Kierownik bierze udział w procesie akceptacji tematów prac dyplomowych. W swoim panelu widzi tematy, które mają status oczekujący na decyzję kierownika.

Kierownik może sortować listę tematów według daty zgłoszenia. Dzięki temu może sprawdzać tematy od najstarszych lub od najnowszych.

Kierownik może zaakceptować temat. Po akceptacji temat nie trafia jeszcze bezpośrednio do listy dostępnej dla studentów, tylko zostaje przekazany dalej do dziekana.

Kierownik może również odrzucić temat. W takim przypadku musi podać powód odrzucenia, który zostaje zapisany w systemie.

### Dziekan

Dziekan odpowiada za końcowy etap akceptacji tematów prac dyplomowych. W swoim panelu widzi tematy, które zostały już zaakceptowane przez kierownika i oczekują na decyzję dziekana.

Dziekan może zaakceptować temat. Po tej decyzji temat otrzymuje status zatwierdzony i staje się dostępny do rezerwacji przez studentów, o ile nie został przekroczony limit tematów dostępnych u danego promotora.

Dziekan może również odrzucić temat. Podczas odrzucania wymagane jest podanie powodu, który zostaje zapisany przy temacie.

---


