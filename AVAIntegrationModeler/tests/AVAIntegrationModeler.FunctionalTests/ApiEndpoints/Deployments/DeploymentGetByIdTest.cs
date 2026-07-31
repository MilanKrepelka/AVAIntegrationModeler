using System.Net.Http.Json;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Infrastructure.Data;

namespace AVAIntegrationModeler.FunctionalTests.ApiEndpoints.Deployments;

/// <summary>
/// Funkční testy pro endpoint GET /Deployments/by-id/{id}.
/// </summary>
[Collection("Sequential")]
public class DeploymentGetByIdTest(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task GetDeploymentById_Existujici_Vrati200()
  {
    var response = await _client.GetAsync($"/Deployments/by-id/{SeedData.Deployment1.Id}");
    response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

    var dto = await response.Content.ReadFromJsonAsync<DeploymentDTO>();
    dto.ShouldNotBeNull();
    dto!.Code.ShouldBe(SeedData.Deployment1.Code);
  }

  [Fact]
  public async Task GetDeploymentById_Neexistujici_Vrati404()
  {
    var response = await _client.GetAsync($"/Deployments/by-id/{Guid.NewGuid()}");
    response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
  }
}
