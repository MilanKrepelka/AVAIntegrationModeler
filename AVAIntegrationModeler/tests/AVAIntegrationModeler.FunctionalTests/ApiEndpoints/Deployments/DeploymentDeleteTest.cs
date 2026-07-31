using AVAIntegrationModeler.Contracts.Deployments;
using AVAIntegrationModeler.Infrastructure.Data;

namespace AVAIntegrationModeler.FunctionalTests.ApiEndpoints.Deployments;

/// <summary>
/// Funkční testy pro endpoint DELETE /Deployments/{code}.
/// </summary>
[Collection("Sequential")]
public class DeploymentDeleteTest(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task DeleteDeployment_Existujici_Vrati204()
  {
    var response = await _client.DeleteAsync($"/Deployments/{SeedData.Deployment3.Code}");
    response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);

    var list = await _client.GetAndDeserializeAsync<DeploymentListResponse>("/Deployments");
    list.Deployments.ShouldNotContain(d => d.Code == SeedData.Deployment3.Code);
  }

  [Fact]
  public async Task DeleteDeployment_Neexistujici_Vrati404()
  {
    var response = await _client.DeleteAsync("/Deployments/NEEXISTUJICI");
    response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
  }
}
