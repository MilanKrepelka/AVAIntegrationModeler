using AVAIntegrationModeler.API.Areas;
using AVAIntegrationModeler.Contracts.Areas;
using AVAIntegrationModeler.Infrastructure.Data;

namespace AVAIntegrationModeler.FunctionalTests.ApiEndpoints.Areas;

/// <summary>
/// Funkční testy pro endpoint GET /Areas.
/// </summary>
[Collection("Sequential")]
public class AreaListTest(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task GetAreas_ReturnsFiveSeededAreas()
  {
    var result = await _client.GetAndDeserializeAsync<AreaListResponse>("/Areas");
    result.Areas.Count.ShouldBe(5);
  }

  [Fact]
  public async Task GetAreas_ContainsSeedArea_SALES()
  {
    var result = await _client.GetAndDeserializeAsync<AreaListResponse>("/Areas");
    result.Areas.ShouldContain(a => a.Code == "SALES");
  }
}
