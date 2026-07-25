# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run

```powershell
# Build entire solution
dotnet build AVAIntegrationModeler.sln

# Run the API (listens on http://0.0.0.0:5005 by default)
dotnet run --project src/AVAIntegrationModeler.API

# Run the Blazor UI
dotnet run --project src/AVAIntegrationModeler.Web.SyncfusionApp

# Run via .NET Aspire (orchestrates both)
dotnet run --project src/AVAIntegrationModeler.AspireHost
```

## Testing

```powershell
# Run all tests
dotnet test AVAIntegrationModeler.sln

# Run a specific test project
dotnet test tests/AVAIntegrationModeler.FunctionalTests
dotnet test tests/AVAIntegrationModeler.IntegrationTests
dotnet test src/AVAIntegrationModeler.Domain.Test

# Run a single test class or method
dotnet test --filter "FullyQualifiedName~ScenarioAddTest"
```

## EF Migrations

Run from the `src/AVAIntegrationModeler.API` directory:

```powershell
dotnet ef migrations add MIGRATIONNAME -c AppDbContext -p ../AVAIntegrationModeler.Infrastructure/AVAIntegrationModeler.Infrastructure.csproj -s AVAIntegrationModeler.API.csproj -o Data/Migrations

dotnet ef database update -c AppDbContext -p ../AVAIntegrationModeler.Infrastructure/AVAIntegrationModeler.Infrastructure.csproj -s AVAIntegrationModeler.API.csproj
```

## NuGet Sources

`ASOL.*` packages come from the private Azure Artifacts feed configured in `nuget.config`. You need access to `https://pkgs.dev.azure.com/avaspace/feed/_packaging/feed/nuget/v3/index.json` to restore these packages.

## Architecture

This is a **Clean Architecture / DDD** solution built on [Ardalis.CleanArchitecture](https://github.com/ardalis/CleanArchitecture). Dependency direction: `API → UseCases → Domain ← Infrastructure`.

### Projects

| Project | Role |
|---|---|
| `Domain` | Aggregates, entities, value objects, domain events, specifications, domain interfaces |
| `UseCases` | MediatR commands/queries, DTOs, query service interfaces, mappers |
| `Infrastructure` | EF Core (`AppDbContext`), repository implementation (`EfRepository`), query services, migrations, email |
| `API` | FastEndpoints (REPR pattern), Swagger, startup wiring |
| `Contracts` | Shared request/response DTOs, enums, options — referenced by API, UseCases, and AVAPlace |
| `AVAPlace` | Integration layer to the external ASOL DataService (tenant-aware HTTP client) |
| `Web.SyncfusionApp` | Blazor Server UI using Syncfusion components |
| `Localization` | Localized text resources |
| `ServiceDefaults` | .NET Aspire shared service defaults (OpenTelemetry, health checks) |

### Key domain concepts

- **Scenario** — integration scenario connecting an input Feature to an output Feature; identified by `Code` (string) and `Id` (Guid)
- **Feature** — integration feature composed of included sub-features and data models
- **DataModel / DataModelField** — data schema definitions
- **IntegrationsMap / IntegrationMapItem** — maps integration scenarios to areas
- **Area** — organizational grouping

### Datasource enum

All query services and handlers accept a `Datasource` parameter (`Database` or `AVAPlace`). This determines whether data is fetched from the local SQLite database or from the external ASOL DataService via `AVAPlace`. The `AVAPlace` project's `IntegrationDataProvider` handles the multi-tenant HTTP calls to the DataService.

### CQRS pattern

- **Commands** (mutating): `Create/Update/DeleteScenario`, `Create/Update/DeleteContributor` — use repository (`IRepository<T>` from `Ardalis.Specification`)
- **Queries** (read-only): handled by dedicated `IXxxQueryService` interfaces (defined in `UseCases`, implemented in `Infrastructure` or `AVAPlace`), bypassing the repository pattern for efficiency
- Query services implement `ICacheableQueryService` with `InvalidateCache(Datasource)` for cache management

### API endpoint naming

Endpoints follow FastEndpoints REPR pattern. Each endpoint is a class ending in `Endpoint` inside a folder named after the aggregate (e.g. `API/Scenarios/Create.cs`). Related request, response, and validator types live in the same folder with the operation name as prefix (e.g. `Create.CreateScenarioRequest.cs`).

### Validation

Validation occurs at two levels:
1. **FastEndpoints validators** on request types (FluentValidation, in `API`)
2. **Guard clauses** (`Ardalis.GuardClauses`) inside domain entity setters
3. **Domain validation services** (`IDomainEntityValidationService`) for cross-entity rules, registered in `InfrastructureServiceExtensions`

Results flow as `Ardalis.Result<T>` — endpoints map `ResultStatus.Invalid` → 400, `ResultStatus.Conflict` → 409.

### LocalizedValue

`LocalizedValue` is a value object with `CzechValue` and `EnglishValue` properties. It is defined as a DTO in `Contracts.DTO.LocalizedValue` and extended in `Domain.ValueObjects.LocalizedValue`. Used for all user-facing text on domain entities (Name, Description).
