using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.Scenarios;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.API.Scenarios;

namespace AVAIntegrationModeler.FunctionalTests.ApiEndpoints.Scenarios;

[Collection("Sequential")]
public class ScenarioDeleteTest(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task ReturnsThreeScenarios()
  {
    var result = await _client.GetAndDeserializeAsync<ScenarioListResponse>("/Scenarios");

    result.Scenarios.Count.ShouldBe(3);

    _ = await _client.DeleteAsync(DeleteScenarioRequest.BuildRoute(Datasource.Database, SeedData.Scenario3.Code));

    var resultAfterDelete = await _client.GetAndDeserializeAsync<ScenarioListResponse>("/Scenarios");

    resultAfterDelete.Scenarios.Count.ShouldBe(2);

  }
}
