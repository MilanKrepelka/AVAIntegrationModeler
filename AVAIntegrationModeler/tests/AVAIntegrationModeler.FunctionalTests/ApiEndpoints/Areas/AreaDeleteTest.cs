using AVAIntegrationModeler.API.Areas;
using AVAIntegrationModeler.Contracts.Areas;
using AVAIntegrationModeler.Infrastructure.Data;

namespace AVAIntegrationModeler.FunctionalTests.ApiEndpoints.Areas;

/// <summary>
/// Funkční testy pro endpoint DELETE /Areas/{Datasource}/{AreaCode}.
/// </summary>
[Collection("Sequential")]
public class AreaDeleteTest(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task DeleteArea_ExistingArea_Returns204AndAreaRemovedFromList()
  {
    var initial = await _client.GetAndDeserializeAsync<AreaListResponse>("/Areas");
    var initialCount = initial.Areas.Count;

    var deleteRoute = DeleteAreaRequest.BuildRoute(Contracts.Datasource.Database, SeedData.Area5.Code);
    var response = await _client.DeleteAsync(deleteRoute);
    response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);

    var after = await _client.GetAndDeserializeAsync<AreaListResponse>("/Areas");
    after.Areas.Count.ShouldBe(initialCount - 1);
    after.Areas.ShouldNotContain(a => a.Code == SeedData.Area5.Code);
  }

  [Fact]
  public async Task DeleteArea_NonExistingArea_Returns404()
  {
    var deleteRoute = DeleteAreaRequest.BuildRoute(Contracts.Datasource.Database, "NONEXISTENT_CODE");
    var response = await _client.DeleteAsync(deleteRoute);
    response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
  }
}
