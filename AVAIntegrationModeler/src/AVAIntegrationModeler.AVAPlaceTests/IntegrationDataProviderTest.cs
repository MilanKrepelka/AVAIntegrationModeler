using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.AVAPlaceTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace AVAIntegrationModeler.AVAPlaceTests;

public class IntegrationDataProvider : TestBed<Fixtures.AVAPlaceDemoFixture>
{
  public IntegrationDataProvider(ITestOutputHelper testOutputHelper, AVAPlaceDemoFixture fixture) : base(testOutputHelper, fixture)
  {
  }

  /// <summary>
  /// Test který načte integrační scénáře
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task GetIntegrationScenariosAsyncTest()
  {
    var integrationDataProvider = this._fixture.GetServiceProvider(_testOutputHelper).GetRequiredService<IIntegrationDataProvider>();

    var result = await integrationDataProvider.GetScenariosAsync(CancellationToken.None);
    result.ShouldNotBeNull();
    result.ShouldNotBeEmpty();
  }

  /// <summary>
  /// Test který načte integrační feature.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task GetFeaturesSummaryAsyncTest()
  {
    var integrationDataProvider = this._fixture.GetServiceProvider(_testOutputHelper).GetRequiredService<IIntegrationDataProvider>();

    var result = await integrationDataProvider.GetFeaturesSummaryAsync(CancellationToken.None);
    result.ShouldNotBeNull();
    result.ShouldNotBeEmpty();
  }

  /// <summary>
  /// Test který načte integrační feature.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task GetDataModelsSummaryAsync()
  {
    var integrationDataProvider = this._fixture.GetServiceProvider(_testOutputHelper).GetRequiredService<IIntegrationDataProvider>();

    var result = await integrationDataProvider.GetDataModelsSummaryAsync(CancellationToken.None);
    result.ShouldNotBeNull();
    result.ShouldNotBeEmpty();
  }

  /// <summary>
  /// Test který načte integrační feature.
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task GetDataModelsAsync()
  {
    var integrationDataProvider = this._fixture.GetServiceProvider(_testOutputHelper).GetRequiredService<IIntegrationDataProvider>();

    var result = await integrationDataProvider.GetDataModelsAsync(CancellationToken.None);
    
    result.ShouldNotBeNull();
    result.ShouldNotBeEmpty();
    result.ShouldAllBe(x => x.Fields != null && x.Fields.Count() > 1);
  }
}
