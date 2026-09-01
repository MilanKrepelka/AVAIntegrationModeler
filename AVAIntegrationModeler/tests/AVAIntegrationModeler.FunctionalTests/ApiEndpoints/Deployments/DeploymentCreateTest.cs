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
  public async Task CreateDeployment_SetsLastSaveDateTime()
  {
    var before = DateTime.UtcNow.AddSeconds(-1);

    var req = new CreateDeploymentRequest
    {
      Deployment = new DeploymentDTO
      {
        Id = Guid.NewGuid(),
        Code = "TEST_CREATE_DATES",
        Name = "Test Create Dates",
        DataModelIds = new List<Guid>()
      }
    };

    await _client.PostAsJsonAsync("/Deployments", req);

    var list = await _client.GetAndDeserializeAsync<DeploymentListResponse>("/Deployments");
    var created = list.Deployments.FirstOrDefault(d => d.Code == "TEST_CREATE_DATES");
    created.ShouldNotBeNull();
    created!.LastSaveDateTime.ShouldNotBeNull();
    created.LastSaveDateTime!.Value.ShouldBeGreaterThan(before);
    created.LastDeploymentDateTime.ShouldBeNull();
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
