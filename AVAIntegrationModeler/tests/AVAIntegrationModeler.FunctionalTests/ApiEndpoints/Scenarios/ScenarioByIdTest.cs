using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.API.Scenarios;

namespace AVAIntegrationModeler.FunctionalTests.ApiEndpoints;

[Collection("Sequential")]
public class ScenarioByIdTest(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task Returns_Scenarios3()
  {
    var result = await _client.GetAndDeserializeAsync<ScenarioRecord>(GetScenarioByIdRequest.BuildRoute(SeedData.Scenario3.Id));
    
    result.Name.ShouldBe(SeedData.Scenario3.Name);
    result.Id.ShouldBe(SeedData.Scenario3.Id);
  }
}
