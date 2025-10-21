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

    var result = await integrationDataProvider.GetIntegrationScenariosAsync(CancellationToken.None);
    result.ShouldNotBeNull();
    result.ShouldNotBeEmpty();
  }
}
