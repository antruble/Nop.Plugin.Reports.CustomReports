# NopCommerce plugin: Custom reports - README

## Plugin Áttekintés
A **CustomReports** plugin egy olyan bővítmény, amelyben új riportokat lehet hozzáadni az admin felülethez.


---

## Új riport létrehozásának menete

### 1. Modellek létrehozása
- `Models\(ReportGroupName)\ReportName` mappában létrehozzuk a riporthoz tartozó `ReportNameReportModel` és `ReportNameListModel`-t
  
- `Models\SearchModels` mappában létrehozzuk a riporthoz tartozó `ReportNameSearchModel`-t, de fel is használhatunk egy már definiált **Search modelt**-t
---

### 2. View létrehozása
- `Views\(ReportGroupName)\ReportName` mappában létrehozzuk a riporthoz tartozó `ReportName` nézetet (egy létező riport nézetet átmásolunk és módosítjuk)
- Létező view módosítása:
  - Viewhez tartozó `SearchModel`-t módosítjuk a riporthoz tartozó `ReportNameSearchModel`-re
  - Fejlécben szereplő beállításokat átírjuk az új riportnak megfelelően
  - Cím átírása
  - A `ReportNameSearchModel`-nek megfelelően módosítjuk a `.search-body` div tartalmát
  - `@await Html.PartialAsync("Table", new DataTablesModel { ... })` részben az `UrlRead = new DataUrl("FetchReport", "ReportControllerName", null) `-nél a `ReportControllerName` a riport controller-ének neve szerepeljen

---

### 3. `ReportsModelFactory` kiegészítése
- `FetchReportNameDataAsync` definiálása a többi riport mintájára
- Szükség esetén segédfüggvényt definiálhatunk a `CustomReportService`-ben
  
---

### 4. Controller létrehozása
- Többi report mintájára `ReportNameController` létrehozása
  - A létrehozott controller a `BaseReportController<ReportNameSearchModel, ReportNameListModel, ReportNameReportModel>`-ból származzon
  - A konstruktorban módosítsuk a `"~/Plugins/Reports.CustomReports/Views/ReportName/ReportName.cshtml"` részt a riporthoz tartozó nézet útvonalára

---

### 5. `BaseReportFactory` kiegészítése
- Ha új search model-t hoztunk létre, akkor regisztráljuk a `BuildSearchModelAsync` metódusban a többi riport mintájára
- A `FetchReportDataAsync` metódus kiegészítése a többi riport mintájára

---

### 6. `CustomReports.cs` kiegészítése
- A `GetReportsMenuItems` metódusban szereplő lista kiegészítése az új riporttal a többi riport mintájára.

---

