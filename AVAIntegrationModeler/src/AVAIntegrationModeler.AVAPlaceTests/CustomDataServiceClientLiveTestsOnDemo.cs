using System.Threading;
using System.Threading.Tasks;
using ASOL.Core.Paging.Contracts.Filters;
using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.AVAPlace.Options;
using AVAIntegrationModeler.AVAPlaceTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace AVAIntegrationModeler.AVAPlaceTests;

/// <summary>
/// Integrační testy <see cref="ICustomDataServiceClient"/> proti AVAPlace demo prostředí.
/// Vyžaduje připojení k internetu a platnou konfiguraci v <c>appsettings.demo.json</c>.
/// </summary>
public class CustomDataServiceClientLiveTests : TestBed<AVAPlaceDemoFixture>
{
  public CustomDataServiceClientLiveTests(ITestOutputHelper testOutputHelper, AVAPlaceDemoFixture fixture)
    : base(testOutputHelper, fixture)
  {
  }

  /// <summary>
  /// Volání <c>GetDataModelsAsync</c> přes bázového klienta musí vrátit neprázdný seznam datových modelů.
  /// Ověřuje, že klient je správně nakonfigurován, autentizace funguje a základní HTTP komunikace probíhá.
  /// </summary>
  [Fact]
  public async Task GetDataModels_ReturnsNonEmptyList()
  {
    var serviceProvider = _fixture.GetServiceProvider(_testOutputHelper);
    var options = serviceProvider.GetRequiredService<IOptions<AVAPlaceOptions>>();
    var tenantId = options.Value.TenantId;

    var result = await ServiceRuntimeTenantContext.ExecuteInContextAsync<ICustomDataServiceClient, object>(
      serviceProvider, tenantId, async client =>
      {
        var models = await client.GetDataModelsAsync(new PagingFilter(), CancellationToken.None);
        return models;
      });

    result.ShouldNotBeNull();
    var list = result as System.Collections.IEnumerable;
    list.ShouldNotBeNull();
    list.Cast<object>().ShouldNotBeEmpty();
  }

  /// <summary>
  /// Volání <c>GetUnifiedDataAsync</c> musí vrátit neprázdný seznam unifikovaných záznamů.
  /// </summary>
  [Fact]
  public async Task GetUnifiedDataAsync_ReturnsNonEmptyList()
  {
    var serviceProvider = _fixture.GetServiceProvider(_testOutputHelper);
    var options = serviceProvider.GetRequiredService<IOptions<AVAPlaceOptions>>();
    var tenantId = options.Value.TenantId;

    var result = await ServiceRuntimeTenantContext.ExecuteInContextAsync<ICustomDataServiceClient, object>(
      serviceProvider, tenantId, async client =>
      {
        var models = await client.GetUnifiedDataAsync(Guid.Parse("35d31a32-1189-4e22-8e33-0a7d1128983c"), CancellationToken.None);
        return models;
      });

    result.ShouldNotBeNull();
    var list = result as System.Collections.IEnumerable;
    list.ShouldNotBeNull();
    list.Cast<object>().ShouldNotBeEmpty();
  }

  /// <summary>
  /// Volání <c>CreateMetadataVersionAsync</c> musí vrátit neprázdný kód verze.
  /// Ověřuje, že endpoint <c>Process/CreateMetadataVersion</c> je dostupný a autentizace funguje.
  /// </summary>
  [Fact]
  public async Task CreateMetadataVersionAsync_ReturnsNonEmptyVersionCode()
  {
    var serviceProvider = _fixture.GetServiceProvider(_testOutputHelper);
    var options = serviceProvider.GetRequiredService<IOptions<AVAPlaceOptions>>();
    var tenantId = options.Value.TenantId;

    var versionCode = await ServiceRuntimeTenantContext.ExecuteInContextAsync<ICustomDataServiceClient, string>(
      serviceProvider, tenantId,
      client => client.CreateMetadataVersionAsync(CancellationToken.None));

    versionCode.ShouldNotBeNullOrWhiteSpace();
  }

  /// <summary>
  /// Importuje unifikovaná data ze souboru <c>qd-OrganizationUnitType.json</c> do DataService na demo prostředí.
  /// Tok: <c>CreateMetadataVersionAsync</c> → <c>ImportUnifiedDataAsync</c> s <c>allowUpdate=true</c>, <c>allowChangeExternalId=true</c>.
  /// Test selže, pokud import vyvolá výjimku nebo server vrátí chybový status.
  /// </summary>
  [Fact]
  public async Task ImportUnifiedDataAsync_OrganizationUnitType_ImportsSuccessfully()
  {
    var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "UnifiedData", "qd-OrganizationUnitType.json");
    File.Exists(filePath).ShouldBeTrue($"Testovací soubor nenalezen: {filePath}");

    var json = await File.ReadAllTextAsync(filePath, TestContext.Current.CancellationToken);
    var jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);

    var serviceProvider = _fixture.GetServiceProvider(_testOutputHelper);
    var options = serviceProvider.GetRequiredService<IOptions<AVAPlaceOptions>>();
    var tenantId = options.Value.TenantId;

    await ServiceRuntimeTenantContext.ExecuteInContextAsync<ICustomDataServiceClient, object?>(
      serviceProvider, tenantId, async client =>
      {
        var versionCode = await client.CreateMetadataVersionAsync(CancellationToken.None);
        versionCode.ShouldNotBeNullOrWhiteSpace();

        using var stream = new MemoryStream(jsonBytes);
        await client.ImportUnifiedDataAsync("OrganizationUnitType", versionCode, allowUpdate: true, allowChangeExternalId: true, stream, CancellationToken.None);

        return null;
      });
  }

  /// <summary>
  /// Importuje soubor <c>dm-OrganizationUnit.json</c> do DataService na demo prostředí.
  /// Tok: <c>CreateMetadataVersionAsync</c> → <c>ImportDataModelAsync</c> s <c>allowUpdate=true</c>.
  /// Test selže, pokud import vyvolá výjimku nebo server vrátí chybový status.
  /// </summary>
  [Fact]
  public async Task ImportDataModelAsync_OrganizationUnit_ImportsSuccessfully()
  {
    var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "Models", "dm-OrganizationUnit.json");
    File.Exists(filePath).ShouldBeTrue($"Testovací soubor nenalezen: {filePath}");

    var serviceProvider = _fixture.GetServiceProvider(_testOutputHelper);
    var options = serviceProvider.GetRequiredService<IOptions<AVAPlaceOptions>>();
    var tenantId = options.Value.TenantId;

    // File.ReadAllText automaticky odstraní UTF-8 BOM, který server odmítá
    var json = await File.ReadAllTextAsync(filePath, TestContext.Current.CancellationToken);
    var jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);

    await ServiceRuntimeTenantContext.ExecuteInContextAsync<ICustomDataServiceClient, object?>(
      serviceProvider, tenantId, async client =>
      {
        var versionCode = await client.CreateMetadataVersionAsync(CancellationToken.None);
        versionCode.ShouldNotBeNullOrWhiteSpace();

        using var stream = new MemoryStream(jsonBytes);
        await client.ImportDataModelAsync(versionCode, allowUpdate: true, stream, CancellationToken.None);

        return null;
      });
  }
}
