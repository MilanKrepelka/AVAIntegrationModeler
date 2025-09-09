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
using Xunit.Abstractions;
using Xunit.Microsoft.DependencyInjection.Abstracts;



namespace AVAIntegrationModeler.IntegrationTests.Data.SqlLite.Scenario;

/// <summary>
/// Představuje testovací třídu pro ověření přidání scénářů v kontextu EF SQL klienta.
/// </summary>
/// <remarks>
/// Tato třída je navržena pro použití s testovacím frameworkem xUnit a závisí na 
/// Fixtures.EfSqlClientTestFixture, který poskytuje potřebné nastavení testu a závislosti.
/// </remarks>
public class ScenarioAddTest : TestBed<EfSqlClientTestFixture>
{
    /// <summary>
    /// Inicializuje novou instanci třídy <see cref="ScenarioAddTest"/>.
    /// </summary>
    /// <param name="testOutputHelper">Pomocník pro výstup testů.</param>
    /// <param name="fixture">Testovací fixture poskytující závislosti.</param>
    public ScenarioAddTest(ITestOutputHelper testOutputHelper, EfSqlClientTestFixture fixture) : base(testOutputHelper, fixture)
    {
    }

    /// <summary>
    /// Ověřuje, že přidání jednoho scénáře nevyvolá chybu.
    /// </summary>
    [Fact]
    public async Task Add_Single_Scenatio_Not_Fails_Test()
    {
        var repository = this._fixture.GetServiceProvider(this._testOutputHelper).GetRequiredService<IRepository<AVAIntegrationModeler.Core.ScenarioAggregate.Scenario>>();

        var context = this._fixture.GetServiceProvider(this._testOutputHelper).GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();

        await repository.AddAsync(SeedData.Scenario1);
    }
}
