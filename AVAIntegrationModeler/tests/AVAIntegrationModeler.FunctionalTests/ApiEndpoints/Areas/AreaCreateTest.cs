using AVAIntegrationModeler.API.Areas;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.Areas;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Infrastructure.Data;

namespace AVAIntegrationModeler.FunctionalTests.ApiEndpoints.Areas;

/// <summary>
/// Funkční testy pro endpoint POST /Areas.
/// </summary>
[Collection("Sequential")]
public class AreaCreateTest(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task CreateArea_ValidRequest_Returns201AndAreaAppearsInList()
  {
    var newArea = new AreaDTO
    {
      Id = Guid.NewGuid(),
      Code = "TEST_CREATE",
      Name = "Test Create Area"
    };

    var createRequest = new CreateAreaRequest
    {
      Datasource = Datasource.Database,
      Area = newArea
    };

    var response = await _client.PostAsJsonAsync("/Areas", createRequest);
    response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);

    var list = await _client.GetAndDeserializeAsync<AreaListResponse>("/Areas");
    list.Areas.ShouldContain(a => a.Code == "TEST_CREATE");
  }

  [Fact]
  public async Task CreateArea_DuplicateCode_Returns409()
  {
    var duplicateArea = new AreaDTO
    {
      Id = Guid.NewGuid(),
      Code = SeedData.Area1.Code,
      Name = "Duplicate Area"
    };

    var createRequest = new CreateAreaRequest
    {
      Datasource = Datasource.Database,
      Area = duplicateArea
    };

    var response = await _client.PostAsJsonAsync("/Areas", createRequest);
    response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Conflict);
  }
}
