using AVAIntegrationModeler.API.Areas;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Infrastructure.Data;

namespace AVAIntegrationModeler.FunctionalTests.ApiEndpoints.Areas;

/// <summary>
/// Funkční testy pro endpoint GET /Areas/{Datasource}/{AreaCode}.
/// </summary>
[Collection("Sequential")]
public class AreaGetByCodeTest(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task GetAreaByCode_ExistingArea_ReturnsCorrectArea()
  {
    var result = await _client.GetAndDeserializeAsync<AreaDTO>(
      GetAreaByCodeRequest.BuildRoute(Datasource.Database, SeedData.Area2.Code));

    result.ShouldNotBeNull();
    result.Code.ShouldBe(SeedData.Area2.Code);
    result.Id.ShouldBe(SeedData.Area2.Id);
  }

  [Fact]
  public async Task GetAreaByCode_NonExistingArea_Returns404()
  {
    var response = await _client.GetAsync(
      GetAreaByCodeRequest.BuildRoute(Datasource.Database, "NONEXISTENT_CODE"));
    response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
  }
}
