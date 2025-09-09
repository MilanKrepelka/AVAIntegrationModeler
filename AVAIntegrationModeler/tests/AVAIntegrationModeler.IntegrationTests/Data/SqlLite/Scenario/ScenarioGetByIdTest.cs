using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVAIntegrationModeler.Core.ScenarioAggregate;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.IntegrationTests.Data.SqlLite.Fixtures;
using AVAIntegrationModeler.UseCases.Contributors.List;
using Castle.Components.DictionaryAdapter.Xml;
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;



namespace AVAIntegrationModeler.IntegrationTests.Data.SqlLite.Scenario;

/// <summary>
/// Představuje testovací třídu pro ověření přidání scénářů v kontextu EF SQL klienta.
/// </summary>
/// <remarks>Tato třída je navržena pro použití s testovacím frameworkem xUnit a závisí na 
/// Fixtures.EfSqlClientTestFixture, který poskytuje potřebné nastavení testu a závislosti.</remarks>
[Collection("Sequential")]
public class ScenarioGetByIdTest : TestBed<EfSqlClientTestFixture>
{
  public ScenarioGetByIdTest(ITestOutputHelper testOutputHelper, EfSqlClientTestFixture fixture) : base(testOutputHelper, fixture)
  {
  }
  /// <summary>
  /// Testuje přidání jednoho scénáře do databáze a ověřuje, že operace proběhne bez chyby.
  /// </summary>
  /// <remarks>
  /// Test ověřuje, že po přidání scénáře lze tento scénář získat zpět podle jeho ID.
  /// Používá se EF SQL klient a testovací data ze třídy SeedData.
  /// </remarks>
  [Fact]
  public async Task Add_Single_Scenatio_Not_Fails_Test()
  {
    var repository = this._fixture.GetServiceProvider(this._testOutputHelper).GetRequiredService<IRepository<AVAIntegrationModeler.Core.ScenarioAggregate.Scenario>>();

    var context = this._fixture.GetServiceProvider(this._testOutputHelper).GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();

    await repository.AddAsync(SeedData.Scenario1);
    var result = await repository.GetByIdAsync(SeedData.Scenario1.Id);
    result!.Id.ShouldBe(SeedData.Scenario1.Id);
  }
}
