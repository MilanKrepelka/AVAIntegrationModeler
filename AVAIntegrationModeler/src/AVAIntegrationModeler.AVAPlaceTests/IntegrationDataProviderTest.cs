using System.Text.Json;
using System.Text.Json.Serialization;
using Ardalis.GuardClauses;
using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.AVAPlaceTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace AVAIntegrationModeler.AVAPlaceTests;

public class IntegrationDataProviderTest : TestBed<Fixtures.AVAPlaceDemoFixture>
{
  public IntegrationDataProviderTest(ITestOutputHelper testOutputHelper, AVAPlaceDemoFixture fixture) : base(testOutputHelper, fixture)
  {
  }

  /// <summary>
  /// Test který načte integrační scénáře
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task GetIntegrationScenariosTest()
  {
    var integrationDataProvider = this._fixture.GetServiceProvider(_testOutputHelper).GetRequiredService<IIntegrationDataProvider>();

    var result = await integrationDataProvider.GetScenarios(CancellationToken.None);
    result.ShouldNotBeNull();
    result.ShouldNotBeEmpty();
  }

  /// <summary>
  /// Test který načte integrační scénář
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task GetIntegrationScenarioTest()
  {
    var integrationDataProvider = this._fixture.GetServiceProvider(_testOutputHelper).GetRequiredService<IIntegrationDataProvider>();

    var scenarios = await integrationDataProvider.GetScenarios(CancellationToken.None);

    var result = await integrationDataProvider.GetScenario(scenarios.ElementAt(0).Id, CancellationToken.None);
    result.ShouldNotBeNull();
    
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

    // Serialize the result to JSON for debugging/inspection
    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
      ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    string json;
    try
    {
      json = JsonSerializer.Serialize(result, options);
    }
    catch (Exception ex)
    {
      _testOutputHelper.WriteLine($"Serialization failed: {ex}");
      throw;
    }

    _testOutputHelper.WriteLine(json);

    result.ShouldNotBeNull();
    result.ShouldNotBeEmpty();
  }

 

  /// <summary>
  /// Test který načte integrační mapy
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task GetIntegrationMapSummaryAsync()
  {
    var integrationDataProvider = this._fixture.GetServiceProvider(_testOutputHelper).GetRequiredService<IIntegrationDataProvider>();

    var result = await integrationDataProvider.GetIntegrationMapSummaryAsync(CancellationToken.None);
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

    var options = new JsonSerializerOptions
    {
      WriteIndented = true,
      DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
      ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    string json;
    try
    {
      json = JsonSerializer.Serialize(result, options);
    }
    catch (Exception ex)
    {
      _testOutputHelper.WriteLine($"Serialization failed: {ex}");
      throw;
    }

    _testOutputHelper.WriteLine(json);

    result.ShouldNotBeNull();
    result.ShouldNotBeEmpty();
    result.ShouldAllBe(x => x.Fields != null && x.Fields.Count() > 1);
  }

  [Fact]
  public async Task GetFeatureSummary_Returns_Any_Result()
  {
    var integrationDataProvider = this._fixture.GetServiceProvider(_testOutputHelper).GetRequiredService<IIntegrationDataProvider>();

    var features = await integrationDataProvider.GetFeaturesAsync(CancellationToken.None);
    var result = await integrationDataProvider.GetFeatureSummary(features.ElementAt(0).Id, CancellationToken.None);
    result.ShouldNotBeNull();
  }

  [Fact]
  public async Task GetFeatureSummary_ByCode_Returns_Any_Result()
  {
    var integrationDataProvider = this._fixture.GetServiceProvider(_testOutputHelper).GetRequiredService<IIntegrationDataProvider>();

    var features = await integrationDataProvider.GetFeaturesAsync(CancellationToken.None);
    var result = await integrationDataProvider.GetFeatureSummary(features.ElementAt(0).Code, CancellationToken.None);
    result.ShouldNotBeNull();
  }

  [Fact]
  public async Task GetFeatureSummary_Throws_Exception_When_Feature_Not_Found()
  {
    var integrationDataProvider = this._fixture.GetServiceProvider(_testOutputHelper).GetRequiredService<IIntegrationDataProvider>();

    await Should.ThrowAsync<NotFoundException>(async () =>
    {
      await integrationDataProvider.GetFeatureSummary(Guid.NewGuid(), CancellationToken.None);
    });
  }

  [Fact]
  public async Task GetScenario_By_Id_Returns_Any_Result()
  {
    var integrationDataProvider = this._fixture.GetServiceProvider(_testOutputHelper).GetRequiredService<IIntegrationDataProvider>();

    var scenario = await integrationDataProvider.GetScenario(Guid.Parse("f2252456-c7a7-41e0-a5a1-13e4a508e57b"), CancellationToken.None);
    scenario.ShouldNotBeNull();

    scenario.InputFeatureId.HasValue.ShouldBeTrue();
    scenario.InputFeatureSummary.ShouldNotBeNull();
    scenario.OutputFeatureId.HasValue.ShouldBeTrue();
    scenario.OutputFeatureSummary.ShouldNotBeNull();
  }

  [Fact]
  public async Task GetScenario_By_Code_Returns_Any_Result()
  {
    var integrationDataProvider = this._fixture.GetServiceProvider(_testOutputHelper).GetRequiredService<IIntegrationDataProvider>();

    var scenario = await integrationDataProvider.GetScenario("BankingProvider", CancellationToken.None);
    scenario.ShouldNotBeNull();

    scenario.InputFeatureId.HasValue.ShouldBeTrue();
    scenario.InputFeatureSummary.ShouldNotBeNull();
    scenario.OutputFeatureId.HasValue.ShouldBeTrue();
    scenario.OutputFeatureSummary.ShouldNotBeNull();
  }

  [Fact]
  public async Task GetFeature_ByCode_Returns_Any_Result()
  {
    var integrationDataProvider = this._fixture.GetServiceProvider(_testOutputHelper).GetRequiredService<IIntegrationDataProvider>();

    var features = await integrationDataProvider.GetFeaturesAsync(CancellationToken.None);
    var result = await integrationDataProvider.GetFeature(features.ElementAt(0).Code, CancellationToken.None);
    result.ShouldNotBeNull();
  }

  [Fact]
  public async Task GetFeature_Throws_Exception_When_Feature_Not_Found()
  {
    var integrationDataProvider = this._fixture.GetServiceProvider(_testOutputHelper).GetRequiredService<IIntegrationDataProvider>();

    await Should.ThrowAsync<NotFoundException>(async () =>
    {
      await integrationDataProvider.GetFeature(Guid.NewGuid(), CancellationToken.None);
    });
  }
}
