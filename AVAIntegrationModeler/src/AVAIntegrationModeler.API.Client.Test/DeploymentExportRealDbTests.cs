using System.IO.Compression;
using System.Text.Json;
using AVAIntegrationModeler.API.Serialization;
using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.AVAPlace.Mapping;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Infrastructure.Data;
using AVAIntegrationModeler.Infrastructure.Data.Queries;
using AVAIntegrationModeler.UseCases.Areas;
using AVAIntegrationModeler.UseCases.DataModelRecords;
using AVAIntegrationModeler.UseCases.DataModels;
using AVAIntegrationModeler.UseCases.DataModelRecords.Export;
using AVAIntegrationModeler.UseCases.Deployments;
using AVAIntegrationModeler.UseCases.Deployments.Export;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AVAIntegrationModeler.API.Client.Test;

/// <summary>
/// Explorační testy exportu nasazení přímo z reálné app.db.
/// Nevyžaduje spuštění API — volá handler přímo přes DI kontejner.
/// Výsledný ZIP je uložen na plochu pro ruční inspekci.
/// </summary>
public class DeploymentExportRealDbTests
{
  private static readonly string DbPath = Path.GetFullPath(Path.Combine(
    Path.GetDirectoryName(typeof(DeploymentExportRealDbTests).Assembly.Location)!,
    "..", "..", "..", "..", "..",
    "src", "AVAIntegrationModeler.API", "app.db"));

  private static IServiceProvider BuildServiceProvider()
  {
    var services = new ServiceCollection();
    services.AddLogging();
    services.AddMemoryCache();
    // AppDbContext nevolá base(options), takže DI options jsou ignorovány a OnConfiguring vždy otevírá
    // "Data Source=app.db" relativně k CWD. Používáme podtřídu s přepsaným OnConfiguring a absolutní cestou.
    services.AddScoped<AppDbContext>(_ => new RealDbAppDbContext(DbPath));
    // Stub pro IIntegrationDataProvider — handler volá pouze Datasource.Database cesty, AVAPlace se nepoužije
    services.AddScoped<IIntegrationDataProvider, StubIntegrationDataProvider>();
    services.AddScoped<IDeploymentsQueryService, DeploymentsQueryService>();
    services.AddScoped<IDataModelQueryService, DataModelsQueryService>();
    services.AddScoped<IDataModelRecordQueryService, DataModelRecordsQueryService>();
    services.AddScoped<IAreasQueryService, AreasQueryService>();
    return services.BuildServiceProvider();
  }

  /// <summary>
  /// Podtřída AppDbContext, která přepisuje OnConfiguring tak, aby použila absolutní cestu k reálné app.db.
  /// Nutná proto, že AppDbContext nevolá base(options) a ignoruje DbContextOptions z DI.
  /// </summary>
  private sealed class RealDbAppDbContext : AppDbContext
  {
    private readonly string _absoluteDbPath;

    public RealDbAppDbContext(string absoluteDbPath)
    {
      _absoluteDbPath = absoluteDbPath;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (!optionsBuilder.IsConfigured)
        optionsBuilder.UseSqlite($"Data Source={_absoluteDbPath}");
    }
  }

  /// <summary>Stub IIntegrationDataProvider — všechny AVAPlace metody vyhodí NotImplementedException.</summary>
  private sealed class StubIntegrationDataProvider : IIntegrationDataProvider
  {
    public Task<IEnumerable<ScenarioDTO>> GetScenarios(CancellationToken ct = default) => throw new NotImplementedException();
    public Task<ScenarioDTO> GetScenario(Guid id, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<ScenarioDTO> GetScenario(string code, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<bool> CreateScenario(ScenarioDTO s, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<bool> UpdateScenario(ScenarioDTO s, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<bool> DeleteScenario(string code, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<FeatureSummaryDTO> GetFeatureSummary(Guid id, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<FeatureDTO> GetFeature(string code, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<FeatureDTO> GetFeature(Guid id, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<FeatureSummaryDTO> GetFeatureSummary(string code, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<IEnumerable<FeatureSummaryDTO>> GetFeaturesSummaryAsync(CancellationToken ct = default) => throw new NotImplementedException();
    public Task<IEnumerable<FeatureDTO>> GetFeaturesAsync(CancellationToken ct = default) => throw new NotImplementedException();
    public Task<IEnumerable<DataModelDTO>> GetDataModelsAsync(CancellationToken ct = default) => throw new NotImplementedException();
    public Task<IEnumerable<DataModelSummaryDTO>> GetDataModelsSummaryAsync(CancellationToken ct = default) => throw new NotImplementedException();
    public Task<IEnumerable<IntegrationMapSummaryDTO>> GetIntegrationMapSummaryAsync(CancellationToken ct = default) => throw new NotImplementedException();
    public Task<IEnumerable<DataModelRecordDTO>> GetDataModelRecordsAsync(Guid modelId, CancellationToken ct = default) => throw new NotImplementedException();
    public Task<DataModelDTO?> GetDataModelByIdAsync(Guid modelId, CancellationToken ct = default) => throw new NotImplementedException();
  }

  /// <summary>
  /// Exportuje nasazení DoploymentTestovacichModelu a uloží ZIP na Desktop.
  /// </summary>
  [Fact]
  public async Task ExportDeployment_DoploymentTestovacichModelu_SavesZipToDesktop()
  {
    const string deploymentCode = "DoploymentTestovacichModelu";

    Assert.True(File.Exists(DbPath), $"Reálná app.db nenalezena: {DbPath}");

    await using var sp = (ServiceProvider)BuildServiceProvider();
    await using var scope = sp.CreateAsyncScope();

    var handler = new ExportDeploymentHandler(
      scope.ServiceProvider.GetRequiredService<IDeploymentsQueryService>(),
      scope.ServiceProvider.GetRequiredService<IDataModelQueryService>(),
      scope.ServiceProvider.GetRequiredService<IDataModelRecordQueryService>(),
      scope.ServiceProvider.GetRequiredService<IAreasQueryService>());

    var result = await handler.Handle(new ExportDeploymentQuery(deploymentCode), CancellationToken.None);

    Assert.True(result.IsSuccess, $"Handler selhal se stavem: {result.Status}");

    // Sestavení ZIP archívu (replika logiky z ExportDeploymentEndpoint)
    using var zipStream = new MemoryStream();
    using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
    {
      foreach (var entry in result.Value.Entries)
      {
        var payload = entry.Data is DataModelDTO model
          ? (object)DataModelMapper.MapToDefinition(model)
          : entry.Data is List<DataModelRecordDTO> records
            ? DataModelRecordExportMapper.MapToExport(records)
            : entry.Data;
        var zipEntry = archive.CreateEntry(entry.FileName, CompressionLevel.Optimal);
        await using var entryStream = zipEntry.Open();
        await JsonSerializer.SerializeAsync(entryStream, payload, ExportJsonOptions.Instance, CancellationToken.None);
      }
    }

    var bytes = zipStream.ToArray();
    Assert.True(bytes.Length > 0, "Export vrátil prázdné pole bytů.");

    var outPath = Path.Combine(
      Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
      $"{deploymentCode}-export.zip");
    await File.WriteAllBytesAsync(outPath, bytes);

    // Výpis obsahu ZIPu pro inspekci
    zipStream.Position = 0;
    using var zip = new ZipArchive(zipStream, ZipArchiveMode.Read);
    foreach (var e in zip.Entries)
      Console.WriteLine($"  {e.FullName} ({e.Length} B)");

    Assert.True(zip.Entries.Count > 0, "ZIP neobsahuje žádné soubory.");
  }
}
