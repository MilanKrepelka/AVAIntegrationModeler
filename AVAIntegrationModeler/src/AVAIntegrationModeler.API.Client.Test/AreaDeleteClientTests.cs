using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging.Abstractions;

namespace AVAIntegrationModeler.API.Client.Test;

/// <summary>
/// Integrační testy mazání oblasti (Area) přes API — ověřují, že smazání oblasti
/// nastaví AreaId na null u DataModelů, které na ni odkazovaly.
/// </summary>
public class AreaDeleteClientTests : IClassFixture<AVAIntegrationModelerAPIFactory>
{
  private readonly AVAIntegrationModelerAPIFactory _factory;

  public AreaDeleteClientTests(AVAIntegrationModelerAPIFactory factory)
  {
    _factory = factory;
  }

  private IAVAIntegrationModelerApiClient CreateClient()
  {
    var http = _factory.CreateClient(new WebApplicationFactoryClientOptions
    {
      BaseAddress = new Uri("http://0.0.0.0:5005")
    });
    return new AVAIntegrationModelerApiClient(http, new TestHttpClientFactory(http), NullLogger<AVAIntegrationModelerApiClient>.Instance);
  }

  private async Task<(Guid Id, string Code)> CreateAreaAsync(IAVAIntegrationModelerApiClient client)
  {
    var code = $"AREA-DEL-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    var area = new AreaDTO { Id = Guid.NewGuid(), Code = code, Name = "Oblast pro test mazání" };
    var result = await client.CreateArea(Datasource.Database, area, CancellationToken.None);
    Assert.True(result.IsSuccess, $"Nepodařilo se vytvořit oblast: {string.Join(", ", result.Errors)}");
    return (area.Id, code);
  }

  private async Task<Guid> CreateDataModelAsync(IAVAIntegrationModelerApiClient client, Guid? areaId)
  {
    var dm = new DataModelDTO
    {
      Id = Guid.NewGuid(),
      Code = $"MODEL-DEL-{Guid.NewGuid().ToString()[..8].ToUpper()}",
      Name = "Model pro test mazání oblasti",
      IsAggregateRoot = false,
      AreaId = areaId,
      Fields = []
    };
    var result = await client.CreateDataModel(Datasource.Database, dm, CancellationToken.None);
    Assert.True(result.IsSuccess, $"Nepodařilo se vytvořit DataModel: {string.Join(", ", result.Errors)}");
    return result.Value;
  }

  /// <summary>
  /// Smazání existující oblasti musí vrátit úspěch.
  /// </summary>
  [Fact]
  public async Task DeleteArea_ExistingArea_ReturnsSuccess()
  {
    var client = CreateClient();
    var (_, code) = await CreateAreaAsync(client);

    var result = await client.DeleteArea(Datasource.Database, code, CancellationToken.None);

    Assert.True(result.IsSuccess, $"DeleteArea selhal: {string.Join(", ", result.Errors)}");
  }

  /// <summary>
  /// Smazání neexistující oblasti musí vrátit chybu (NotFound).
  /// </summary>
  [Fact]
  public async Task DeleteArea_NonExistentArea_ReturnsError()
  {
    var client = CreateClient();
    var nonExistentCode = $"NEEXISTUJE-{Guid.NewGuid().ToString()[..8].ToUpper()}";

    var result = await client.DeleteArea(Datasource.Database, nonExistentCode, CancellationToken.None);

    Assert.False(result.IsSuccess);
  }

  /// <summary>
  /// Po smazání oblasti musí DataModel, který na ni odkazoval, mít AreaId null.
  /// </summary>
  [Fact]
  public async Task DeleteArea_WithReferencingDataModel_SetsDataModelAreaIdToNull()
  {
    var client = CreateClient();
    var (areaId, areaCode) = await CreateAreaAsync(client);
    var modelId = await CreateDataModelAsync(client, areaId);

    var deleteResult = await client.DeleteArea(Datasource.Database, areaCode, CancellationToken.None);
    Assert.True(deleteResult.IsSuccess, $"DeleteArea selhal: {string.Join(", ", deleteResult.Errors)}");

    var fetchedModel = await client.GetDataModel(Datasource.Database, modelId, CancellationToken.None);

    Assert.NotNull(fetchedModel);
    Assert.Null(fetchedModel.AreaId);
  }

  /// <summary>
  /// DataModel bez oblasti nesmí být smazáním jiné oblasti ovlivněn.
  /// </summary>
  [Fact]
  public async Task DeleteArea_UnrelatedDataModelWithoutArea_IsNotAffected()
  {
    var client = CreateClient();
    var (_, areaCode) = await CreateAreaAsync(client);
    var modelId = await CreateDataModelAsync(client, areaId: null);

    var deleteResult = await client.DeleteArea(Datasource.Database, areaCode, CancellationToken.None);
    Assert.True(deleteResult.IsSuccess, $"DeleteArea selhal: {string.Join(", ", deleteResult.Errors)}");

    var fetchedModel = await client.GetDataModel(Datasource.Database, modelId, CancellationToken.None);

    Assert.NotNull(fetchedModel);
    Assert.Null(fetchedModel.AreaId);
  }
}
