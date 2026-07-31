using System.Net.Http.Json;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Infrastructure.Data;

namespace AVAIntegrationModeler.FunctionalTests.ApiEndpoints.Deployments;

/// <summary>
/// Funkční testy pro endpoint GET /Deployments/{code}.
/// </summary>
[Collection("Sequential")]
public class DeploymentGetByCodeTest(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task GetDeploymentByCode_Existujici_Vrati200()
  {
    var response = await _client.GetAsync($"/Deployments/{SeedData.Deployment1.Code}");
    response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

    var dto = await response.Content.ReadFromJsonAsync<DeploymentDTO>();
    dto.ShouldNotBeNull();
    dto!.Id.ShouldBe(SeedData.Deployment1.Id);
  }

  [Fact]
  public async Task GetDeploymentByCode_Neexistujici_Vrati404()
  {
    var response = await _client.GetAsync("/Deployments/NEEXISTUJICI");
    response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
  }
}
