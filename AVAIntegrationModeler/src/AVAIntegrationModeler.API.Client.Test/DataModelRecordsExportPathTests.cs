using System.IO.Compression;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging.Abstractions;

namespace AVAIntegrationModeler.API.Client.Test;

/// <summary>
/// Integrační testy exportu záznamů DataModelů — ověřují, že <c>POST /DataModelRecords/export</c>
/// ukládá jednotlivé záznamy do ZIP archívu pod cestou <c>dataobjects/{Area.Code}/qd-{Model.Name}.json</c>,
/// kde je adresář a název souboru odvozen od DataModelu, k němuž záznam patří.
/// </summary>
public class DataModelRecordsExportPathTests : IClassFixture<AVAIntegrationModelerAPIFactory>
{
  private readonly AVAIntegrationModelerAPIFactory _factory;

  public DataModelRecordsExportPathTests(AVAIntegrationModelerAPIFactory factory)
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

  private async Task<Guid> CreateAreaAsync(IAVAIntegrationModelerApiClient client, string code)
  {
    var area = new AreaDTO { Id = Guid.NewGuid(), Code = code, Name = "Testovací oblast" };
    var result = await client.CreateArea(Datasource.Database, area, CancellationToken.None);
    Assert.True(result.IsSuccess, $"Nepodařilo se vytvořit oblast: {string.Join(", ", result.Errors)}");
    return area.Id;
  }

  private async Task<Guid> CreateDataModelAsync(IAVAIntegrationModelerApiClient client, string name, Guid? areaId)
  {
    var dm = new DataModelDTO
    {
      Id = Guid.NewGuid(),
      Code = $"EXP-REC-{Guid.NewGuid().ToString()[..8].ToUpper()}",
      Name = name,
      IsAggregateRoot = false,
      AreaId = areaId,
      Fields = []
    };
    var result = await client.CreateDataModel(Datasource.Database, dm, CancellationToken.None);
    Assert.True(result.IsSuccess, $"Nepodařilo se vytvořit DataModel: {string.Join(", ", result.Errors)}");
    return result.Value;
  }

  private async Task<Guid> CreateRecordAsync(IAVAIntegrationModelerApiClient client, Guid modelId)
  {
    var record = new DataModelRecordDTO
    {
      Id = Guid.NewGuid(),
      ModelId = modelId,
      ExternalId = $"EXT-{Guid.NewGuid().ToString()[..8].ToUpper()}",
      Fields = []
    };
    var result = await client.CreateDataModelRecord(Datasource.Database, record, CancellationToken.None);
    Assert.True(result.IsSuccess, $"Nepodařilo se vytvořit záznam: {string.Join(", ", result.Errors)}");
    return result.Value;
  }

  /// <summary>
  /// Záznam modelu patřícího do oblasti musí být v ZIPu na cestě dataobjects/{Area.Code}/qd-{Model.Name}.json.
  /// </summary>
  [Fact]
  public async Task ExportDataModelRecords_ModelWithArea_UsesAreaCodeAndModelNameInPath()
  {
    var client = CreateClient();
    var areaCode = $"AR-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    var areaId = await CreateAreaAsync(client, areaCode);
    var modelName = $"Model-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    var modelId = await CreateDataModelAsync(client, modelName, areaId);
    var recordId = await CreateRecordAsync(client, modelId);

    var bytes = await client.ExportDataModelRecords(Datasource.Database, [recordId], CancellationToken.None);

    using var ms = new MemoryStream(bytes);
    using var zip = new ZipArchive(ms, ZipArchiveMode.Read);
    var expectedPath = $"dataobjects/{areaCode}/qd-{modelName}.json";
    Assert.Contains(zip.Entries, e => e.FullName == expectedPath);
  }

  /// <summary>
  /// Záznam modelu bez oblasti musí být v ZIPu pod adresářem "bez-oblasti".
  /// </summary>
  [Fact]
  public async Task ExportDataModelRecords_ModelWithoutArea_UsesFallbackDirectory()
  {
    var client = CreateClient();
    var modelName = $"Model-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    var modelId = await CreateDataModelAsync(client, modelName, areaId: null);
    var recordId = await CreateRecordAsync(client, modelId);

    var bytes = await client.ExportDataModelRecords(Datasource.Database, [recordId], CancellationToken.None);

    using var ms = new MemoryStream(bytes);
    using var zip = new ZipArchive(ms, ZipArchiveMode.Read);
    var expectedPath = $"dataobjects/bez-oblasti/qd-{modelName}.json";
    Assert.Contains(zip.Entries, e => e.FullName == expectedPath);
  }

  /// <summary>
  /// Dva záznamy téhož modelu musí mít v ZIPu shodnou cestu (kolize je akceptovaným chováním
  /// — poslední zpracovaný záznam v archívu přepíše předchozí).
  /// </summary>
  [Fact]
  public async Task ExportDataModelRecords_TwoRecordsSameModel_ProduceSamePath()
  {
    var client = CreateClient();
    var areaCode = $"AR-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    var areaId = await CreateAreaAsync(client, areaCode);
    var modelName = $"Model-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    var modelId = await CreateDataModelAsync(client, modelName, areaId);
    var record1Id = await CreateRecordAsync(client, modelId);
    var record2Id = await CreateRecordAsync(client, modelId);

    var bytes = await client.ExportDataModelRecords(Datasource.Database, [record1Id, record2Id], CancellationToken.None);

    using var ms = new MemoryStream(bytes);
    using var zip = new ZipArchive(ms, ZipArchiveMode.Read);
    var expectedPath = $"dataobjects/{areaCode}/qd-{modelName}.json";
    Assert.Equal(2, zip.Entries.Count(e => e.FullName == expectedPath));
  }
}
