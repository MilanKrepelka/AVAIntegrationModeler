using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVAIntegrationModeler.Core.ContributorAggregate;
using AVAIntegrationModeler.Core.ScenarioAggregate;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.IntegrationTests.Data.SqlLite.Fixtures;
using AVAIntegrationModeler.UseCases.Contributors.List;
using Castle.Components.DictionaryAdapter.Xml;
using Xunit.Microsoft.DependencyInjection.Abstracts;



namespace AVAIntegrationModeler.IntegrationTests.Data.SqlLite.Scenarios;

/// <summary>
/// Představuje testovací třídu pro ověření přidání scénářů v kontextu EF SQL klienta.
/// </summary>
/// <remarks>
/// Tato třída je navržena pro použití s testovacím frameworkem xUnit a závisí na 
/// Fixtures.EfSqlClientTestFixture, který poskytuje potřebné nastavení testu a závislosti.
/// </remarks>
/// <remarks>
/// Inicializuje novou instanci třídy <see cref="ScenarioAddTest"/>.
/// </remarks>
/// <param name="testOutputHelper">Pomocník pro výstup testů.</param>
/// <param name="fixture">Testovací fixture poskytující závislosti.</param>
public class ScenarioUpdateTest(ITestOutputHelper testOutputHelper, EfSqlClientTestFixture fixture) : TestBed<EfSqlClientTestFixture>(testOutputHelper, fixture)
{

  /// <summary>
  /// Ověřuje, že přidání jednoho scénáře nevyvolá chybu.
  /// </summary>
  [Fact]
  public async Task Update_Single_Scenatio_Not_Fails_Test()
  {
    var repository = _fixture.GetServiceProvider(_testOutputHelper).GetRequiredService<IRepository<Core.ScenarioAggregate.Scenario>>();

    var context = _fixture.GetServiceProvider(_testOutputHelper).GetRequiredService<AppDbContext>();
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
    await repository.AddAsync(SeedData.Scenario1);
    
    var foundedScenario = await repository.GetByIdAsync(SeedData.Scenario1.Id);
    string newCode = "NewCode"+Guid.NewGuid();
    
    foundedScenario!.SetCode(newCode);
    await repository.UpdateAsync(foundedScenario);

    var updatedScenario = await repository.GetByIdAsync(SeedData.Scenario1.Id);

    updatedScenario!.Code.ShouldBe(newCode);

  }
}
