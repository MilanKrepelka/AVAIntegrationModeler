using AVAIntegrationModeler.API.Areas;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Infrastructure.Data;

namespace AVAIntegrationModeler.FunctionalTests.ApiEndpoints.Areas;

/// <summary>
/// Funkční testy pro endpoint GET /Areas/by-id/{Datasource}/{AreaId}.
/// </summary>
[Collection("Sequential")]
public class AreaGetByIdTest(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task GetAreaById_ExistingArea_ReturnsCorrectArea()
  {
    var result = await _client.GetAndDeserializeAsync<AreaDTO>(
      GetAreaByIdRequest.BuildRoute(SeedData.Area1.Id));

    result.ShouldNotBeNull();
    result.Id.ShouldBe(SeedData.Area1.Id);
    result.Code.ShouldBe(SeedData.Area1.Code);
  }

  [Fact]
  public async Task GetAreaById_NonExistingArea_Returns404()
  {
    var response = await _client.GetAsync(GetAreaByIdRequest.BuildRoute(Guid.NewGuid()));
    response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
  }
}
