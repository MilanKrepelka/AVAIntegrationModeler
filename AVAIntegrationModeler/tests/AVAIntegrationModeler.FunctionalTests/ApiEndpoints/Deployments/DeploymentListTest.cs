using AVAIntegrationModeler.Contracts.Deployments;
using AVAIntegrationModeler.Infrastructure.Data;

namespace AVAIntegrationModeler.FunctionalTests.ApiEndpoints.Deployments;

/// <summary>
/// Funkční testy pro endpoint GET /Deployments.
/// </summary>
[Collection("Sequential")]
public class DeploymentListTest(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task GetDeployments_VratiSeededNasazeni()
  {
    var result = await _client.GetAndDeserializeAsync<DeploymentListResponse>("/Deployments");
    result.Deployments.Count.ShouldBe(3);
  }

  [Fact]
  public async Task GetDeployments_ObsahujeDeployment1()
  {
    var result = await _client.GetAndDeserializeAsync<DeploymentListResponse>("/Deployments");
    result.Deployments.ShouldContain(d => d.Code == SeedData.Deployment1.Code);
  }
}
