using System.IO.Compression;
using System.Text.Json;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging.Abstractions;

namespace AVAIntegrationModeler.API.Client.Test;

/// <summary>
/// Integrační testy exportu nasazení — ověřují, že <c>POST /Deployments/{code}/export</c>
/// vrátí ZIP archív s DataModely a jejich záznamy.
/// </summary>
public class DeploymentExportClientTests : IClassFixture<AVAIntegrationModelerAPIFactory>
{
  private readonly AVAIntegrationModelerAPIFactory _factory;

  public DeploymentExportClientTests(AVAIntegrationModelerAPIFactory factory)
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

  private static DataModelDTO NewDataModel(string? code = null, Guid? areaId = null) => new DataModelDTO
  {
    Id = Guid.NewGuid(),
    Code = code ?? $"DEP-EXP-{Guid.NewGuid().ToString()[..8].ToUpper()}",
    Name = "Testovací DataModel pro export nasazení",
    Description = "DataModel vytvořený pro účely exportního testu.",
    IsAggregateRoot = false,
    AreaId = areaId,
    Fields = []
  };

  private async Task<Guid> CreateAreaAsync(IAVAIntegrationModelerApiClient client, string code)
  {
    var area = new AreaDTO { Id = Guid.NewGuid(), Code = code, Name = "Testovací oblast" };
    var result = await client.CreateArea(Datasource.Database, area, CancellationToken.None);
    Assert.True(result.IsSuccess, $"Nepodařilo se vytvořit oblast: {string.Join(", ", result.Errors)}");
    return area.Id;
  }

  private static DeploymentDTO NewDeployment(string? code = null, List<Guid>? modelIds = null) => new DeploymentDTO
  {
    Id = Guid.NewGuid(),
    Code = code ?? $"DEP-{Guid.NewGuid().ToString()[..8].ToUpper()}",
    Name = "Testovací nasazení",
    DataModelIds = modelIds ?? []
  };

  private async Task<Guid> CreateDataModelAsync(IAVAIntegrationModelerApiClient client, string? code = null, Guid? areaId = null)
  {
    var result = await client.CreateDataModel(Datasource.Database, NewDataModel(code, areaId), CancellationToken.None);
    Assert.True(result.IsSuccess, $"Nepodařilo se vytvořit DataModel: {string.Join(", ", result.Errors)}");
    return result.Value;
  }

  private async Task<string> CreateDeploymentWithModelAsync(IAVAIntegrationModelerApiClient client, Guid modelId)
  {
    var dto = NewDeployment(modelIds: [modelId]);
    var result = await client.CreateDeployment(dto, CancellationToken.None);
    Assert.True(result.IsSuccess, $"Nepodařilo se vytvořit Deployment: {string.Join(", ", result.Errors)}");
    return dto.Code;
  }

  /// <summary>
  /// Export nasazení s jedním DataModelem vrátí neprázdný ZIP.
  /// </summary>
  [Fact]
  public async Task ExportDeployment_WithValidCode_ReturnsZipBytes()
  {
    var client = CreateClient();
    var modelId = await CreateDataModelAsync(client);
    var code = await CreateDeploymentWithModelAsync(client, modelId);

    var bytes = await client.ExportDeployment(code, CancellationToken.None);

    Assert.NotNull(bytes);
    Assert.True(bytes.Length > 0, "Export vrátil prázdné pole bytů.");
  }

  /// <summary>
  /// ZIP archív exportu musí obsahovat jeden soubor odpovídající vytvořenému DataModelu.
  /// </summary>
  [Fact]
  public async Task ExportDeployment_WithOneModel_ZipContainsOneEntry()
  {
    var client = CreateClient();
    var modelId = await CreateDataModelAsync(client);
    var code = await CreateDeploymentWithModelAsync(client, modelId);

    var bytes = await client.ExportDeployment(code, CancellationToken.None);

    using var ms = new MemoryStream(bytes);
    using var zip = new ZipArchive(ms, ZipArchiveMode.Read);
    Assert.Single(zip.Entries);
  }

  /// <summary>
  /// JSON soubor v ZIP musí obsahovat klíče <c>Definition</c> a <c>Records</c>.
  /// </summary>
  [Fact]
  public async Task ExportDeployment_ZipEntry_ContainsDefinitionAndRecords()
  {
    var client = CreateClient();
    var modelId = await CreateDataModelAsync(client);
    var code = await CreateDeploymentWithModelAsync(client, modelId);

    var bytes = await client.ExportDeployment(code, CancellationToken.None);

    using var ms = new MemoryStream(bytes);
    using var zip = new ZipArchive(ms, ZipArchiveMode.Read);
    var entry = zip.Entries[0];
    using var entryStream = entry.Open();
    var doc = await JsonDocument.ParseAsync(entryStream);
    Assert.True(doc.RootElement.TryGetProperty("Definition", out _), "Chybí klíč 'Definition'.");
    Assert.True(doc.RootElement.TryGetProperty("Records", out _), "Chybí klíč 'Records'.");
  }

  /// <summary>
  /// Záznamy přiřazené k DataModelu musí být zahrnuty v exportu.
  /// </summary>
  [Fact]
  public async Task ExportDeployment_WithDataModelAndRecord_RecordsAreIncluded()
  {
    var client = CreateClient();
    var modelId = await CreateDataModelAsync(client);

    var record = new DataModelRecordDTO
    {
      Id = Guid.NewGuid(),
      ModelId = modelId,
      ExternalId = $"EXT-{Guid.NewGuid().ToString()[..8].ToUpper()}",
      Fields = []
    };
    var recordResult = await client.CreateDataModelRecord(Datasource.Database, record, CancellationToken.None);
    Assert.True(recordResult.IsSuccess, $"Nepodařilo se vytvořit záznam: {string.Join(", ", recordResult.Errors)}");

    var code = await CreateDeploymentWithModelAsync(client, modelId);

    var bytes = await client.ExportDeployment(code, CancellationToken.None);

    using var ms = new MemoryStream(bytes);
    using var zip = new ZipArchive(ms, ZipArchiveMode.Read);
    var entry = zip.Entries[0];
    using var entryStream = entry.Open();
    var doc = await JsonDocument.ParseAsync(entryStream);
    var records = doc.RootElement.GetProperty("Records");
    Assert.Equal(JsonValueKind.Array, records.ValueKind);
    Assert.True(records.GetArrayLength() >= 1, "Records pole neobsahuje žádné záznamy.");
  }

  /// <summary>
  /// ZIP entry pro DataModel s oblastí musí být na cestě datamodels/{Area.Code}/dm-{Model.Name}.json.
  /// </summary>
  [Fact]
  public async Task ExportDeployment_ModelWithArea_UsesAreaCodeAndModelNameInPath()
  {
    var client = CreateClient();
    var areaCode = $"AR-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    var areaId = await CreateAreaAsync(client, areaCode);
    var modelName = $"Model-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    var dto = NewDataModel() with { Name = modelName, AreaId = areaId };
    var createResult = await client.CreateDataModel(Datasource.Database, dto, CancellationToken.None);
    Assert.True(createResult.IsSuccess);
    var code = await CreateDeploymentWithModelAsync(client, createResult.Value);

    var bytes = await client.ExportDeployment(code, CancellationToken.None);

    using var ms = new MemoryStream(bytes);
    using var zip = new ZipArchive(ms, ZipArchiveMode.Read);
    var expectedPath = $"datamodels/{areaCode}/dm-{modelName}.json";
    Assert.Contains(zip.Entries, e => e.FullName == expectedPath);
  }

  /// <summary>
  /// ZIP entry pro DataModel bez oblasti musí být pod adresářem "bez-oblasti".
  /// </summary>
  [Fact]
  public async Task ExportDeployment_ModelWithoutArea_UsesFallbackDirectory()
  {
    var client = CreateClient();
    var modelName = $"Model-{Guid.NewGuid().ToString()[..8].ToUpper()}";
    var dto = NewDataModel() with { Name = modelName };
    var createResult = await client.CreateDataModel(Datasource.Database, dto, CancellationToken.None);
    Assert.True(createResult.IsSuccess);
    var code = await CreateDeploymentWithModelAsync(client, createResult.Value);

    var bytes = await client.ExportDeployment(code, CancellationToken.None);

    using var ms = new MemoryStream(bytes);
    using var zip = new ZipArchive(ms, ZipArchiveMode.Read);
    var expectedPath = $"datamodels/bez-oblasti/dm-{modelName}.json";
    Assert.Contains(zip.Entries, e => e.FullName == expectedPath);
  }

  /// <summary>
  /// Export neexistujícího nasazení musí vrátit HTTP 404.
  /// </summary>
  [Fact]
  public async Task ExportDeployment_WithNonExistentCode_ThrowsHttpRequestException()
  {
    var client = CreateClient();
    var nonExistentCode = $"NEEXISTUJE-{Guid.NewGuid()}";

    await Assert.ThrowsAsync<HttpRequestException>(
      () => client.ExportDeployment(nonExistentCode, CancellationToken.None));
  }
}
