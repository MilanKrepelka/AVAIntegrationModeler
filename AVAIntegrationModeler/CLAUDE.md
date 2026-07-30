# CLAUDE.md

Tento soubor poskytuje pokyny pro Claude Code (claude.ai/code) při práci s kódem v tomto repozitáři.

## Sestavení a spuštění

```powershell
# Sestavení celého solution
dotnet build AVAIntegrationModeler.sln

# Spuštění API (naslouchá na http://0.0.0.0:5005)
dotnet run --project src/AVAIntegrationModeler.API

# Spuštění Blazor UI
dotnet run --project src/AVAIntegrationModeler.Web.SyncfusionApp

# Spuštění přes .NET Aspire (orchestruje obojí)
dotnet run --project src/AVAIntegrationModeler.AspireHost
```

## Testování

```powershell
# Spuštění všech testů
dotnet test AVAIntegrationModeler.sln

# Spuštění konkrétního testovacího projektu
dotnet test tests/AVAIntegrationModeler.FunctionalTests
dotnet test tests/AVAIntegrationModeler.IntegrationTests
dotnet test src/AVAIntegrationModeler.Domain.Test

# Spuštění jednoho testu nebo třídy
dotnet test --filter "FullyQualifiedName~ScenarioAddTest"
```

## EF Migrace

Spouštět z adresáře `src/AVAIntegrationModeler.API`:

```powershell
dotnet ef migrations add NAZEV_MIGRACE -c AppDbContext -p ../AVAIntegrationModeler.Infrastructure/AVAIntegrationModeler.Infrastructure.csproj -s AVAIntegrationModeler.API.csproj -o Data/Migrations

dotnet ef database update -c AppDbContext -p ../AVAIntegrationModeler.Infrastructure/AVAIntegrationModeler.Infrastructure.csproj -s AVAIntegrationModeler.API.csproj
```

## NuGet zdroje

Balíčky `ASOL.*` pocházejí z privátního Azure Artifacts feedu nakonfigurovaného v `nuget.config`. Pro obnovení balíčků je potřeba přístup k `https://pkgs.dev.azure.com/avaspace/feed/_packaging/feed/nuget/v3/index.json`.

## Architektura

Řešení je postaveno na **Clean Architecture / DDD** šabloně [Ardalis.CleanArchitecture](https://github.com/ardalis/CleanArchitecture). Směr závislostí: `API → UseCases → Domain ← Infrastructure`.

### Projekty

| Projekt | Role |
|---|---|
| `Domain` | Agregáty, entity, hodnotové objekty, doménové události, specifikace, doménová rozhraní |
| `UseCases` | MediatR příkazy/dotazy, DTO, rozhraní query služeb, mappery |
| `Infrastructure` | EF Core (`AppDbContext`), implementace repozitáře (`EfRepository`), query služby, migrace, email |
| `API` | FastEndpoints (vzor REPR), Swagger, registrace služeb |
| `Contracts` | Sdílená request/response DTO, enumy, options — používáno z API, UseCases i AVAPlace |
| `AVAPlace` | Integrační vrstva na externí ASOL DataService (multi-tenant HTTP klient) |
| `Web.SyncfusionApp` | Blazor Server UI s Syncfusion komponentami |
| `API.Client` | Generovaný HTTP klient pro API — používán v integračních testech a externími konzumenty |
| `Localization` | Lokalizační textové zdroje |
| `ServiceDefaults` | Sdílené výchozí nastavení .NET Aspire (OpenTelemetry, health checks); obsahuje `FluentClientFactory` / `IFluentClientFactory` — abstrakce nad typovanými HTTP klienty, používaná v `AVAPlace` i `API.Client` |

### Klíčové doménové koncepty

- **Scenario** — integrační scénář propojující vstupní Feature s výstupní Feature; identifikován přes `Code` (string) a `Id` (Guid)
- **Feature** — integrační feature složená z vložených sub-featur a datových modelů
- **DataModel / DataModelField** — definice datového schématu
- **IntegrationsMap / IntegrationMapItem** — mapování integračních scénářů na oblasti
- **Area** — organizační seskupení

### Enum Datasource

Všechny query služby a handlery přijímají parametr `Datasource` (`Database` nebo `AVAPlace`). Ten určuje, zda se data čtou z lokální SQLite databáze nebo z externí ASOL DataService přes `AVAPlace`. Multi-tenant HTTP volání zajišťuje `IntegrationDataProvider` v projektu `AVAPlace`.

### Vzor CQRS

- **Příkazy** (mutace): `Create/Update/DeleteScenario`, `Create/Update/DeleteContributor` — používají repozitář (`IRepository<T>` z `Ardalis.Specification`)
- **Dotazy** (jen čtení): zpracovávány dedikovanými rozhraními `IXxxQueryService` (definovanými v `UseCases`, implementovanými v `Infrastructure` nebo `AVAPlace`), obcházejí vzor repozitáře pro efektivitu
- Query služby implementují `ICacheableQueryService` s metodou `InvalidateCache(Datasource)` pro správu cache

### Pojmenování API endpointů

Endpointy sledují vzor FastEndpoints REPR. Každý endpoint je třída končící `Endpoint` ve složce pojmenované podle agregátu (např. `API/Scenarios/Create.cs`). Příslušné request, response a validační typy leží ve stejné složce s prefixem operace (např. `Create.CreateScenarioRequest.cs`).

### Validace

Validace probíhá na dvou úrovních:
1. **FastEndpoints validátory** na request typech (FluentValidation, v `API`)
2. **Guard klauzule** (`Ardalis.GuardClauses`) uvnitř setterů doménových entit
3. **Doménové validační služby** (`IDomainEntityValidationService`) pro pravidla přes více entit, registrované v `InfrastructureServiceExtensions`

Výsledky proudí jako `Ardalis.Result<T>` — endpointy mapují `ResultStatus.Invalid` → 400, `ResultStatus.Conflict` → 409.

`ResultError(string Code, string Message, string? Field)` je sealed record pro typované předávání chyb mezi vrstvami.

### LocalizedValue

`LocalizedValue` je hodnotový objekt s vlastnostmi `CzechValue` a `EnglishValue`. Definován jako DTO v `Contracts.DTO.LocalizedValue` a rozšířen v `Domain.ValueObjects.LocalizedValue`. Používá se pro veškerý uživatelský text na doménových entitách (Name, Description).

### Doménové základní typy

Všechny agregáty dědí z `EntityBase` a implementují `IAggregateRoot` (z `Ardalis.SharedKernel`). Hodnotové objekty dědí z `ValueObject`. `Domain` projekt používá globální using direktivy pro celý Ardalis stack: `GuardClauses`, `Result`, `SharedKernel`, `SmartEnum`, `Specification`, `MediatR`.

### Doménové události

Doménové události dědí z `DomainEventBase`. Handlery jsou MediatR notification handlery umístěné v `Aggregate/Events/` a `Aggregate/Handlers/`. `EventDispatcherInterceptor` (EF Core `SaveChangesInterceptor` v `Infrastructure`) automaticky dispatchuje doménové události po každém `SaveChanges`.

### Specifikace

Query specifikace dědí z `Specification<T>` (`Ardalis.Specification`), např. `ScenarioByIdSpec`, `ScenarioByCodeSpec`, `FeatureByIdSpec`. Používají se v command handlerech pro načítání entit přes `IRepository<T>`.

### Mapování

`IMapper<TDomainEntity, TDTO, TSelf>` v `UseCases` využívá C# 11 statické abstraktní členy rozhraní — `MapToDTO` a `MapToEntity` jsou implementovány jako statické metody na konkrétních mapper třídách. Paralelní `IMapper` existuje také v `Contracts`.

### AVAPlace vrstva

`IIntegrationDataProvider` (v `Contracts/AVAPlace/`) je primární kontrakt pro načítání dat z externí ASOL DataService. Implementován třídou `IntegrationDataProvider` v projektu `AVAPlace`. `CustomDataServiceClient` zajišťuje samotná HTTP volání. Multi-tenant kontext proudí přes `IntegrationDataProvider`.

### Testovací projekty

Testy jsou rozloženy mezi kolocované projekty (v `src/`) a separátní projekty (v `tests/`):

| Projekt | Co testuje |
|---|---|
| `Domain.Test` (`src/`) | Doménové agregáty (Area, Contributor, DataModel, Feature, Scenario), ValueObjects |
| `Infrastructure.Test` (`src/`) | EF datová vrstva přes SQLite fixture; query služby (ListFeatures, ListScenarios); DataModelTree |
| `UseCases.Test` (`src/`) | Mapper třídy (AreaMapper, DataModelMapper, FeatureMapper, LocalizedValueMapper) |
| `API.Client.Test` (`src/`) | Klientské integrační testy přes `AVAIntegrationModelerAPIFactory` |
| `AVAPlaceTests` (`src/`) | Live integrační testy proti AVAPlace demo prostředí; mapper třídy a `IntegrationDataProvider` |
| `FunctionalTests` (`tests/`) | API endpointy přes `CustomWebApplicationFactory` (Contributors, Scenarios); AVAPlace HTTP klient |
| `IntegrationTests` (`tests/`) | EF repozitář přes SQLite; Scenario CRUD |
| `UnitTests` (`tests/`) | Doménové agregáty, doménové služby, UseCase handlery, Web mapování, FluentClientFactory |
| `AspireTests` (`tests/`) | Aspire orchestration smoke test |

## UI — Web.SyncfusionApp

Blazor Server aplikace (.NET 9, `InteractiveServer` render mode). Všechny stránky jsou implementovány jako partial třídy se souborem code-behind (`*.razor` + `*.razor.cs`).

### Syncfusion komponenty

Projekt používá [Syncfusion Blazor](https://www.syncfusion.com/blazor-components). Licence se registruje v `Program.cs` přes `SyncfusionLicenseProvider.RegisterLicense(...)`. Komponenty jsou zaregistrovány přes `builder.Services.AddSyncfusionBlazor()`.

Hlavní používané komponenty:

| Komponenta | Balíček | Použití |
|---|---|---|
| `SfGrid<T>` | `Syncfusion.Blazor.Grid` | Seznamy (Scenarios, Features, DataModels) |
| `SfDiagramComponent` + `RadialTree` layout | `Syncfusion.Blazor.Diagram` | Diagram scénáře, strom datového modelu |
| `SfDropDownList<TItem,TValue>` | `Syncfusion.Blazor.DropDowns` | Výběry na formuláři ScenarioEdit |
| `SfTextBox`, `SfTextArea` | `Syncfusion.Blazor.Inputs` | Textové vstupy na formulářích |
| `SfRadioButton`, `SfCheckBox`, `SfButton` | `Syncfusion.Blazor.Buttons` | Akce a přepínače |
| `SfToast` | `Syncfusion.Blazor.Notifications` | Zpětná vazba — pouze na list stránkách (kopírovat ID). **Na edit formulářích nepoužívat** — způsobuje `removeChild` crash při navigaci. |
| `SfToolbar` | `Syncfusion.Blazor.Navigations` | Nástrojová lišta na Scenarios |

Globální `@using Syncfusion.Blazor` a `@using Syncfusion.Blazor.Diagram` jsou v `_Imports.razor`; ostatní namespace se přidávají lokálně.

### Stránky a routy

| Stránka | Route | Popis |
|---|---|---|
| `Features.razor` | `/features` | SfGrid seznam integrčních features |
| `DataModels.razor` | `/datamodels` | SfGrid seznam datových modelů s detail template |
| `DataModelMap.razor` | `/datamodelmap/{modelId:guid}` | Radiální strom referencí datového modelu (SfDiagramComponent) |
| `Scenarios.razor` | `/scenarios` | SfGrid seznam scénářů |
| `ScenarioEdit.razor` | `/scenarioedit/{ds}/{code}` nebo `/scenarioedit/{ds}` | Formulář pro vytvoření / editaci scénáře |
| `ScenariosMap.razor` | `/scenariomap/{ds}/{code}` | Diagram scénáře (SfDiagramComponent, RadialTree) |
| `Areas.razor` | `/areas` | SfGrid seznam oblastí |
| `AreaEdit.razor` | `/areaedit/{code}` nebo `/areaedit` | Formulář pro vytvoření / editaci oblasti (inline alert, bez SfToast) |

Složka `Components/Pages/ComponentsSyncfusion/` (~40 souborů) je **pouze referenční demo** Syncfusion Template Studio — není součástí navigace aplikace.

### Sdílené vzory UI

**`IPageListBase`** — rozhraní pro list stránky s `IsLoading`, `Datasource`, `FilterString`. Implementováno v `Scenarios` a `Features`.

**`DataSourceSelector`** (widget) — dva `SfRadioButton` pro přepínání mezi `Datasource.Database` a `Datasource.AVAPlace`. Parametry: `@bind-Selected`, `OnChanged` (typovaný callback s `Datasource` enum), `LabelText`, `CssClass`.

**`SuccessErrorToast`** (widget) — **NEPOUŽÍVAT na edit formulářích.** `SfToast` způsobuje `removeChild` crash při navigaci pryč ze stránky. Místo toho použít inline Bootstrap alert (`_saveMessage` / `_saveSuccess` state + `<div class="alert alert-success/danger">`).

### Vzor zprávy o uložení na edit formulářích

Každá edit stránka musí zobrazit zprávu o výsledku uložení. Vzor je jednotný pro všechny edit stránky (AreaEdit, DataModelEdit, DataModelRecordEdit, ScenarioEdit):

**code-behind (`.razor.cs`)**:
```csharp
private string? _saveMessage;
private bool _saveSuccess;

private async Task SaveAsync()
{
  // ...
  var result = await _apiClient.CreateXxx(...) / UpdateXxx(...);
  if (_disposed) return;
  _saveSuccess = result.IsSuccess;
  _saveMessage = result.IsSuccess
    ? $"Xxx {dto.Code} byl(a) {(IsNew ? "vytvořen(a)" : "uložen(a)")}."
    : result.IsInvalid()
      ? string.Join(", ", result.ValidationErrors.Select(e => e.ErrorMessage))
      : string.Join(", ", result.Errors);
  // ...
}
```

**šablona (`.razor`)** — umístit před tlačítka Uložit/Zpět:
```razor
@if (_saveMessage is not null)
{
    <div class="alert @(_saveSuccess ? "alert-success" : "alert-danger")" style="margin-top:16px">
        @_saveMessage
    </div>
}
```

### Tlačítko „Přidat" v toolbaru SfGrid

Akce vytvoření nového záznamu **patří dovnitř toolbaru SfGrid** jako `ToolbarItem`, ne jako samostatné `SfButton` mimo grid. Všechny list stránky s odpovídající edit stránkou tento vzor dodržují (Areas, Scenarios, DataModels).

```razor
<SfGrid ...>
    <Syncfusion.Blazor.Navigations.SfToolbar>
        <Syncfusion.Blazor.Navigations.ToolbarItems>
            <Syncfusion.Blazor.Navigations.ToolbarItem
                Text="Přidat oblast"
                PrefixIcon="e-icons e-add"
                Id="AddNew"
                OnClick="AddNew">
            </Syncfusion.Blazor.Navigations.ToolbarItem>
        </Syncfusion.Blazor.Navigations.ToolbarItems>
    </Syncfusion.Blazor.Navigations.SfToolbar>
    ...
</SfGrid>
```

Handler musí mít signaturu `void Handler(Syncfusion.Blazor.Navigations.ClickEventArgs args)`.  
Výjimky, kde tlačítko může zůstat vně: kontextové akce závislé na výběru (např. DataModelRecords — "Přidat záznam" závisí na vybraném modelu).

### Vzor pro list stránky (SfGrid + načítání dat)

**Rendermode**: List stránky s `SfGrid` používají `@rendermode InteractiveServer` (bez prerender). **Nepoužívat `InteractiveServerRenderMode(prerender: true)` na list stránkách** — kombinace prerender + `OnInitializedAsync` způsobuje dvojitou inicializaci a SfGrid nedetekuje změnu dat při navigaci zpět.

**Načítání dat**: Vždy vytvořit novou instanci listu a přiřadit ji (ne mutovat existující):
```csharp
// ✅ SPRÁVNĚ — nová instance, SfGrid detekuje změnu reference
var newList = new List<AreaListViewModel>();
// ... naplnit newList ...
AreasList = newList;

// ❌ ŠPATNĚ — SfGrid nedetekuje mutaci stejné instance
AreasList.Clear();
AreasList.Add(...);
```

**StateHasChanged**: Používat `await InvokeAsync(StateHasChanged)` místo přímého `StateHasChanged()`.

**Grid Refresh**: Po přiřazení nového listu volat `if (Grid != null) await Grid.Refresh()` pro explicitní reload gridu.

**Vzorový LoadItemsAsync** (viz `Areas.razor.cs`, `DataModels.razor.cs`):
```csharp
IsLoading = true;
await InvokeAsync(StateHasChanged);
var newList = new List<TViewModel>();
// ... načíst data, naplnit newList ...
DataList = newList;
// finally:
IsLoading = false;
await InvokeAsync(StateHasChanged);
if (Grid != null) await Grid.Refresh();
```

### ViewModels a mapování

Stránky pracují s vlastními view modely ze složky `ViewModels/List/` (`ScenarioListViewModel`, `DataModelListViewModel`, `FeatureListViewModel`, `IntegrationMapListViewModel`). Mapování z API response DTO na view modely zajišťují třídy ve složce `Mapping/`.

### Přístup k datům

Stránky injektují `IAVAIntegrationModelerApiClient` (z projektu `API.Client`) pro volání API. `DataModelTree` (namespace `AVAIntegrationModeler.Infrastructure`) builduje rekurzivní strom referencí pro `DataModelMap.razor`.

# Pravidla pro implementaci

## Obecná pravidla
- Dodržuj jmenné konvence Microsoftu a strukturu složek dle existujícího řešení.
- Všech kód musí mít dokumentaci v češtině(XML doc) a být pokrytý testy.
- Všechny nové funkce musí být implementovány v souladu s Clean Architecture A DDD principy.
