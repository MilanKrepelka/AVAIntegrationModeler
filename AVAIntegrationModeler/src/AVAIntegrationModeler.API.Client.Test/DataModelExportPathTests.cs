using System.IO.Compression;
using System.Text.Json;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging.Abstractions;

namespace AVAIntegrationModeler.API.Client.Test;

/// <summary>
/// Integrační testy exportu DataModelů — ověřují, že <c>POST /DataModels/export</c>
/// ukládá definici modelu do ZIP archívu pod cestou <c>datamodels/{Area.Code}/dm-{Model.Name}.json</c>
/// a pokud má model záznamy, i tyto záznamy pod cestou <c>dataobjects/{Area.Code}/qd-{Model.Name}.json</c>.
/// </summary>
public class DataModelExportPathTests : IClassFixture<AVAIntegrationModelerAPIFactory>
{
  private readonly AVAIntegrationModelerAPIFactory _factory;

  public DataModelExportPathTests(AVAIntegrationModelerAPIFactory factory)
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
      Code = $"EXP-{Guid.NewGuid().ToString()[..8].ToUpper()}",
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
  /// DataModel patřící do oblasti musí být v ZIPu na cestě datamodels/{Area.Code}/dm-{Model.Name}.json.
  /// </summary>
  [Fact]
  public async Task ExportDataModels_ModelWithArea_UsesAreaCodeAndModelNameInPath()
  {
    var client = CreateClient();
    var areaCode = $"AR-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    var areaId = await CreateAreaAsync(client, areaCode);
    var modelName = $"Model-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    var modelId = await CreateDataModelAsync(client, modelName, areaId);

    var bytes = await client.ExportDataModels(Datasource.Database, [modelId], CancellationToken.None);

    using var ms = new MemoryStream(bytes);
    using var zip = new ZipArchive(ms, ZipArchiveMode.Read);
    var expectedPath = $"datamodels/{areaCode}/dm-{modelName}.json";
    Assert.Contains(zip.Entries, e => e.FullName == expectedPath);
  }

  /// <summary>
  /// DataModel bez oblasti musí být v ZIPu pod adresářem "bez-oblasti".
  /// </summary>
  [Fact]
  public async Task ExportDataModels_ModelWithoutArea_UsesFallbackDirectory()
  {
    var client = CreateClient();
    var modelName = $"Model-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    var modelId = await CreateDataModelAsync(client, modelName, areaId: null);

    var bytes = await client.ExportDataModels(Datasource.Database, [modelId], CancellationToken.None);

    using var ms = new MemoryStream(bytes);
    using var zip = new ZipArchive(ms, ZipArchiveMode.Read);
    var expectedPath = $"datamodels/bez-oblasti/dm-{modelName}.json";
    Assert.Contains(zip.Entries, e => e.FullName == expectedPath);
  }

  /// <summary>
  /// Pokud DataModel má záznamy, musí export obsahovat i soubor se záznamy na cestě
  /// dataobjects/{Area.Code}/qd-{Model.Name}.json vedle definičního souboru.
  /// </summary>
  [Fact]
  public async Task ExportDataModels_ModelWithRecords_IncludesRecordsEntry()
  {
    var client = CreateClient();
    var areaCode = $"AR-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    var areaId = await CreateAreaAsync(client, areaCode);
    var modelName = $"Model-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    var modelId = await CreateDataModelAsync(client, modelName, areaId);
    await CreateRecordAsync(client, modelId);
    await CreateRecordAsync(client, modelId);

    var bytes = await client.ExportDataModels(Datasource.Database, [modelId], CancellationToken.None);

    using var ms = new MemoryStream(bytes);
    using var zip = new ZipArchive(ms, ZipArchiveMode.Read);

    var definitionPath = $"datamodels/{areaCode}/dm-{modelName}.json";
    Assert.Contains(zip.Entries, e => e.FullName == definitionPath);

    var recordsPath = $"dataobjects/{areaCode}/qd-{modelName}.json";
    var recordsEntry = Assert.Single(zip.Entries, e => e.FullName == recordsPath);
    using var doc = await JsonDocument.ParseAsync(recordsEntry.Open());
    Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
    Assert.Equal(2, doc.RootElement.GetArrayLength());
  }

  /// <summary>
  /// Pokud DataModel nemá žádné záznamy, export nesmí obsahovat soubor se záznamy
  /// (žádnou cestu pod dataobjects/) — pouze definiční soubor.
  /// </summary>
  [Fact]
  public async Task ExportDataModels_ModelWithoutRecords_DoesNotIncludeRecordsEntry()
  {
    var client = CreateClient();
    var modelName = $"Model-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    var modelId = await CreateDataModelAsync(client, modelName, areaId: null);

    var bytes = await client.ExportDataModels(Datasource.Database, [modelId], CancellationToken.None);

    using var ms = new MemoryStream(bytes);
    using var zip = new ZipArchive(ms, ZipArchiveMode.Read);

    Assert.Single(zip.Entries);
    Assert.DoesNotContain(zip.Entries, e => e.FullName.StartsWith("dataobjects/"));
  }
}
