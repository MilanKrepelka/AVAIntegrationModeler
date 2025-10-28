using Ardalis.SharedKernel;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.Integration.Test.Data.SqlLite.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace AVAIntegrationModeler.Infrastructure.Test.Data;

/// <summary>
/// Základní třída pro databázové testy s podporou čištění databáze.
/// </summary>
public abstract class BaseDbTests : TestBed<EfSqlClientTestFixture>
{
  protected readonly AppDbContext DbContext;

  protected BaseDbTests(ITestOutputHelper testOutputHelper, EfSqlClientTestFixture fixture)
    : base(testOutputHelper, fixture)
  {
    var serviceProvider = fixture.GetServiceProvider(testOutputHelper);
    DbContext = serviceProvider.GetRequiredService<AppDbContext>();

    // Vytvoření databáze
    DbContext.Database.EnsureCreated();

    // Vyčištění databáze před každým testem
    ClearDatabaseAsync().GetAwaiter().GetResult();
  }

  /// <summary>
  /// Vyčistí všechna data z databáze a resetuje ChangeTracker.
  /// </summary>
  protected async Task ClearDatabaseAsync()
  {
    // Smazání všech dat z tabulek v pořadí kvůli foreign keys
    // Nejdříve závislé tabulky, pak hlavní
    
    // Owned entity collections
    DbContext.DataModelFields.RemoveRange(DbContext.DataModelFields);
    DbContext.DataModelFieldEntityTypeReferences.RemoveRange(DbContext.DataModelFieldEntityTypeReferences);
    
    // Aggregate roots - v pořadí od nejzávislejších
    DbContext.Scenarios.RemoveRange(DbContext.Scenarios);
    DbContext.Features.RemoveRange(DbContext.Features);
    DbContext.DataModels.RemoveRange(DbContext.DataModels);
    DbContext.Areas.RemoveRange(DbContext.Areas);
    DbContext.Contributors.RemoveRange(DbContext.Contributors);

    await DbContext.SaveChangesAsync();

    // Detach všech entit z context pro čistý stav
    DbContext.ChangeTracker.Clear();
  }

  /// <summary>
  /// Vytvoří repository pro daný typ agregátu.
  /// </summary>
  protected EfRepository<T> GetRepository<T>() where T : class, IAggregateRoot
  {
    return new EfRepository<T>(DbContext);
  }
}
