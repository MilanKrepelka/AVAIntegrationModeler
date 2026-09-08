using System.Net.Http.Json;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Contracts.Deployments;
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
  public async Task UpdateDeployment_OdebereDataModely_UloziDoDb()
  {
    // Deployment1 má v seed datech 2 DataModely — po update s prázdným seznamem musí zmizet z DB
    var updated = new DeploymentDTO
    {
      Id = SeedData.Deployment1.Id,
      Code = SeedData.Deployment1.Code,
      Name = SeedData.Deployment1.Name,
      DataModelIds = new List<Guid>()
    };

    var putResponse = await _client.PutAsJsonAsync(
      $"/Deployments/{SeedData.Deployment1.Code}",
      new { DeploymentCode = SeedData.Deployment1.Code, Deployment = updated });
    putResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

    // Ověř, že GET vrátí prázdný seznam DataModelů
    var getResponse = await _client.GetAsync($"/Deployments/{SeedData.Deployment1.Code}");
    getResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
    var dto = await getResponse.Content.ReadFromJsonAsync<DeploymentDTO>();
    dto.ShouldNotBeNull();
    dto!.DataModelIds.ShouldBeEmpty();
  }

  [Fact]
  public async Task UpdateDeployment_PridaDataModely_UloziDoDb()
  {
    // Deployment3 nemá žádné DataModely — přidáme jeden
    var newModelId = SeedData.DataModel1.Id;
    var updated = new DeploymentDTO
    {
      Id = SeedData.Deployment3.Id,
      Code = SeedData.Deployment3.Code,
      Name = SeedData.Deployment3.Name,
      DataModelIds = new List<Guid> { newModelId }
    };

    var putResponse = await _client.PutAsJsonAsync(
      $"/Deployments/{SeedData.Deployment3.Code}",
      new { DeploymentCode = SeedData.Deployment3.Code, Deployment = updated });
    putResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

    // Ověř, že GET vrátí přidaný DataModel
    var getResponse = await _client.GetAsync($"/Deployments/{SeedData.Deployment3.Code}");
    getResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
    var dto = await getResponse.Content.ReadFromJsonAsync<DeploymentDTO>();
    dto.ShouldNotBeNull();
    dto!.DataModelIds.ShouldContain(newModelId);
  }

  [Fact]
  public async Task UpdateDeployment_SetsLastSaveDateTime()
  {
    var before = DateTime.UtcNow.AddSeconds(-1);

    var updated = new DeploymentDTO
    {
      Id = SeedData.Deployment2.Id,
      Code = SeedData.Deployment2.Code,
      Name = "Updated For Date Check",
      DataModelIds = new List<Guid>()
    };

    var response = await _client.PutAsJsonAsync(
      $"/Deployments/{SeedData.Deployment2.Code}",
      new { DeploymentCode = SeedData.Deployment2.Code, Deployment = updated });

    response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);

    var dto = await response.Content.ReadFromJsonAsync<DeploymentDTO>();
    dto.ShouldNotBeNull();
    dto!.LastSaveDateTime.ShouldNotBeNull();
    dto.LastSaveDateTime!.Value.ShouldBeGreaterThan(before);
    dto.LastDeploymentDateTime.ShouldBeNull();
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
