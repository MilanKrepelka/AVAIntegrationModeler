using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.API.Scenarios;

namespace AVAIntegrationModeler.FunctionalTests.ApiEndpoints;

[Collection("Sequential")]
public class ScenarioListTest(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task ReturnsThreeScenarios()
  {
    var result = await _client.GetAndDeserializeAsync<ScenarioListResponse>("/Scenarios");

    result.Scenarios.Count.ShouldBe(3);
    result.Scenarios.ElementAt(2).Name.ShouldBe(SeedData.Scenario1.Name);
    result.Scenarios.ElementAt(1).Name.ShouldBe(SeedData.Scenario2.Name);
    result.Scenarios.ElementAt(0).Name.ShouldBe(SeedData.Scenario3.Name);
  }
}
