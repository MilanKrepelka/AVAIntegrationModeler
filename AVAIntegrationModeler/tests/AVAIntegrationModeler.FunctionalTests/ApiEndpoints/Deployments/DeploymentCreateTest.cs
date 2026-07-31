using AVAIntegrationModeler.API.Deployments;
using AVAIntegrationModeler.Contracts.Deployments;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Infrastructure.Data;

namespace AVAIntegrationModeler.FunctionalTests.ApiEndpoints.Deployments;

/// <summary>
/// Funkční testy pro endpoint POST /Deployments.
/// </summary>
[Collection("Sequential")]
public class DeploymentCreateTest(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task CreateDeployment_ValidniPozadavek_Vrati201()
  {
    var req = new CreateDeploymentRequest
    {
      Deployment = new DeploymentDTO
      {
        Id = Guid.NewGuid(),
        Code = "TEST_CREATE_DEP",
        Name = "Test Create Deployment",
        DataModelIds = new List<Guid>()
      }
    };

    var response = await _client.PostAsJsonAsync("/Deployments", req);
    response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);

    var list = await _client.GetAndDeserializeAsync<DeploymentListResponse>("/Deployments");
    list.Deployments.ShouldContain(d => d.Code == "TEST_CREATE_DEP");
  }

  [Fact]
  public async Task CreateDeployment_DuplikatniKod_Vrati409()
  {
    var req = new CreateDeploymentRequest
    {
      Deployment = new DeploymentDTO
      {
        Id = Guid.NewGuid(),
        Code = SeedData.Deployment1.Code,
        Name = "Duplicate"
      }
    };

    var response = await _client.PostAsJsonAsync("/Deployments", req);
    response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Conflict);
  }
}
