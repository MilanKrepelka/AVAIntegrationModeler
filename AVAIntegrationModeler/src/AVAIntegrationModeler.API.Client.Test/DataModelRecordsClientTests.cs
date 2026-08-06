using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging.Abstractions;

namespace AVAIntegrationModeler.API.Client.Test;

/// <summary>
/// Integrační testy API vrstvy pro metodu <see cref="IAVAIntegrationModelerApiClient.GetDataModelRecords"/>.
/// Ověřují správné fungování načítání záznamů datových modelů přes API klienta.
/// </summary>
public class DataModelRecordsClientTests : IClassFixture<AVAIntegrationModelerAPIFactory>
{
  private readonly AVAIntegrationModelerAPIFactory _factory;

  public DataModelRecordsClientTests(AVAIntegrationModelerAPIFactory factory)
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

  private async Task<Guid> CreateDataModelAsync(IAVAIntegrationModelerApiClient client, string? code = null)
  {
    var dm = new DataModelDTO
    {
      Id = Guid.NewGuid(),
      Code = code ?? $"REC-TEST-{Guid.NewGuid().ToString()[..8].ToUpper()}",
      Name = "Testovací datový model",
      Description = "Model pro testy GetDataModelRecords",
      IsAggregateRoot = false,
      Fields = []
    };
    var result = await client.CreateDataModel(Datasource.Database, dm, CancellationToken.None);
    Assert.True(result.IsSuccess, $"Nepodařilo se vytvořit DataModel: {string.Join(", ", result.Errors)}");
    return result.Value;
  }

  private async Task<Guid> CreateRecordAsync(IAVAIntegrationModelerApiClient client, Guid modelId, string externalId)
  {
    var record = new DataModelRecordDTO
    {
      Id = Guid.NewGuid(),
      ModelId = modelId,
      ExternalId = externalId,
      Fields = []
    };
    var result = await client.CreateDataModelRecord(Datasource.Database, record, CancellationToken.None);
    Assert.True(result.IsSuccess, $"Nepodařilo se vytvořit DataModelRecord: {string.Join(", ", result.Errors)}");
    return result.Value;
  }

  /// <summary>
  /// DataModel bez záznamů — GetDataModelRecords musí vrátit prázdný seznam.
  /// </summary>
  [Fact]
  public async Task GetDataModelRecords_NoRecords_ReturnsEmptyList()
  {
    var client = CreateClient();
    var modelId = await CreateDataModelAsync(client);

    var response = await client.GetDataModelRecords(Datasource.Database, modelId, CancellationToken.None);

    Assert.NotNull(response);
    Assert.Empty(response.Records);
  }

  /// <summary>
  /// Po vytvoření záznamu musí GetDataModelRecords vrátit tento záznam se správným ExternalId a ModelId.
  /// </summary>
  [Fact]
  public async Task GetDataModelRecords_AfterCreate_ReturnsRecord()
  {
    var client = CreateClient();
    var modelId = await CreateDataModelAsync(client);
    var externalId = $"EXT-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    await CreateRecordAsync(client, modelId, externalId);
    
    var response = await client.GetDataModelRecords(Datasource.Database, modelId, CancellationToken.None);

    Assert.NotNull(response);
    var record = Assert.Single(response.Records);
    Assert.Equal(externalId, record.ExternalId);
    Assert.Equal(modelId, record.ModelId);
  }

  /// <summary>
  /// Filtrování podle modelId musí vrátit pouze záznamy daného modelu, nikoli záznamy jiného modelu.
  /// </summary>
  [Fact]
  public async Task GetDataModelRecords_FilterByModelId_ReturnsOnlyMatchingRecords()
  {
    var client = CreateClient();
    var model1Id = await CreateDataModelAsync(client);
    var model2Id = await CreateDataModelAsync(client);

    await CreateRecordAsync(client, model1Id, "EXT-MODEL1-A");
    await CreateRecordAsync(client, model1Id, "EXT-MODEL1-B");
    await CreateRecordAsync(client, model2Id, "EXT-MODEL2-A");

    var response = await client.GetDataModelRecords(Datasource.Database, model1Id, CancellationToken.None);

    Assert.NotNull(response);
    Assert.Equal(2, response.Records.Count);
    Assert.All(response.Records, r => Assert.Equal(model1Id, r.ModelId));
    Assert.DoesNotContain(response.Records, r => r.ExternalId == "EXT-MODEL2-A");
  }

  /// <summary>
  /// Null modelId (bez filtru) musí vrátit záznamy ze všech modelů.
  /// </summary>
  [Fact]
  public async Task GetDataModelRecords_NullModelId_ReturnsRecordsFromAllModels()
  {
    var client = CreateClient();
    var model1Id = await CreateDataModelAsync(client);
    var model2Id = await CreateDataModelAsync(client);

    var extId1 = $"EXT-ALL1-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    var extId2 = $"EXT-ALL2-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    await CreateRecordAsync(client, model1Id, extId1);
    await CreateRecordAsync(client, model2Id, extId2);

    var response = await client.GetDataModelRecords(Datasource.Database, null, CancellationToken.None);

    Assert.NotNull(response);
    // Databáze je sdílená — hledáme naše záznamy v celé sadě výsledků
    Assert.Contains(response.Records, r => r.ExternalId == extId1);
    Assert.Contains(response.Records, r => r.ExternalId == extId2);
  }

  /// <summary>
  /// Záznamy s poli musí být vráceny včetně hodnot polí.
  /// </summary>
  [Fact]
  public async Task GetDataModelRecords_RecordWithFields_ReturnsFields()
  {
    var client = CreateClient();
    var modelId = await CreateDataModelAsync(client);

    var record = new DataModelRecordDTO
    {
      Id = Guid.NewGuid(),
      ModelId = modelId,
      ExternalId = $"EXT-FIELDS-{Guid.NewGuid().ToString()[..8].ToUpper()}",
      Fields =
      [
        new DataModelRecordFieldDTO { Key = "Nazev", IsLocalized = false, StringValue = "Testovací hodnota" },
        new DataModelRecordFieldDTO { Key = "Popis", IsLocalized = true, CzechValue = "Česky", EnglishValue = "English" }
      ]
    };
    var createResult = await client.CreateDataModelRecord(Datasource.Database, record, CancellationToken.None);
    Assert.True(createResult.IsSuccess);

    var response = await client.GetDataModelRecords(Datasource.Database, modelId, CancellationToken.None);

    Assert.NotNull(response);
    var returned = Assert.Single(response.Records);
    Assert.Equal(2, returned.Fields.Count);

    var nazev = returned.Fields.FirstOrDefault(f => f.Key == "Nazev");
    Assert.NotNull(nazev);
    Assert.False(nazev.IsLocalized);
    Assert.Equal("Testovací hodnota", nazev.StringValue);

    var popis = returned.Fields.FirstOrDefault(f => f.Key == "Popis");
    Assert.NotNull(popis);
    Assert.True(popis.IsLocalized);
    Assert.Equal("Česky", popis.CzechValue);
    Assert.Equal("English", popis.EnglishValue);
  }
}
