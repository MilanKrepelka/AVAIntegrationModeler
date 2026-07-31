using System.Net.Http.Json;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Infrastructure.Data;

namespace AVAIntegrationModeler.FunctionalTests.ApiEndpoints.Deployments;

/// <summary>
/// Funkční testy pro endpoint PUT /Deployments/{code}.
/// </summary>
[Collection("Sequential")]
public class DeploymentUpdateTest(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task UpdateDeployment_Existujici_Vrati200()
  {
    var updated = new DeploymentDTO
    {
      Id = SeedData.Deployment2.Id,
      Code = SeedData.Deployment2.Code,
      Name = "Updated Finance Deployment",
      DataModelIds = new List<Guid>()
    };

    var response = await _client.PutAsJsonAsync(
      $"/Deployments/{SeedData.Deployment2.Code}",
      new { DeploymentCode = SeedData.Deployment2.Code, Deployment = updated });

    response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

    var dto = await response.Content.ReadFromJsonAsync<DeploymentDTO>();
    dto.ShouldNotBeNull();
    dto!.Name.ShouldBe("Updated Finance Deployment");
  }

  [Fact]
  public async Task UpdateDeployment_Neexistujici_Vrati404()
  {
    var dto = new DeploymentDTO
    {
      Id = Guid.NewGuid(),
      Code = "NEEXISTUJICI",
      Name = "Test"
    };

    var response = await _client.PutAsJsonAsync(
      "/Deployments/NEEXISTUJICI",
      new { DeploymentCode = "NEEXISTUJICI", Deployment = dto });

    response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
  }
}
