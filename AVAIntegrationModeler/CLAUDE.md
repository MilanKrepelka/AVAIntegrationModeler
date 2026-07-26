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
