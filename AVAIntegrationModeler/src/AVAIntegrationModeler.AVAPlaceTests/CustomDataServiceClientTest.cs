using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ASOL.Core.ApiConnector;
using ASOL.Core.Paging.Contracts.Filters;
using ASOL.DataService.Connector.Options;
using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.AVAPlace.API.Connectors;
using AVAIntegrationModeler.AVAPlace.Options;
using AVAIntegrationModeler.AVAPlaceTests.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit.Microsoft.DependencyInjection.Abstracts;

namespace AVAIntegrationModeler.AVAPlaceTests;

/// <summary>
/// Unit testy pro vlastní metody <see cref="CustomDataServiceClient"/>.
/// HTTP volání jsou zachycena pomocí <see cref="FakeHttpMessageHandler"/> — bez připojení k AVAPlace.
/// </summary>
public class CustomDataServiceClientTests
{
  // ---------------------------------------------------------------------------
  // GetDataAgentByCodeAsync
  // ---------------------------------------------------------------------------

  /// <summary>
  /// Prázdný kód agenta musí způsobit ArgumentNullException ještě před HTTP voláním.
  /// </summary>
  [Fact]
  public async Task GetDataAgentByCodeAsync_EmptyCode_ThrowsArgumentNullException()
  {
    var client = CreateClient(new HttpResponseMessage(HttpStatusCode.OK));

    await Should.ThrowAsync<ArgumentNullException>(() =>
      client.GetDataAgentByCodeAsync(string.Empty, acceptNotFound: false, CancellationToken.None));
  }

  /// <summary>
  /// Null kód agenta musí způsobit ArgumentNullException.
  /// </summary>
  [Fact]
  public async Task GetDataAgentByCodeAsync_NullCode_ThrowsArgumentNullException()
  {
    var client = CreateClient(new HttpResponseMessage(HttpStatusCode.OK));

    await Should.ThrowAsync<ArgumentNullException>(() =>
      client.GetDataAgentByCodeAsync(null!, acceptNotFound: false, CancellationToken.None));
  }

  /// <summary>
  /// Pokud server vrátí 404 a <c>acceptNotFound=true</c>, výsledek je <c>null</c>.
  /// </summary>
  [Fact]
  public async Task GetDataAgentByCodeAsync_NotFound_AcceptNotFound_ReturnsNull()
  {
    var client = CreateClient(new HttpResponseMessage(HttpStatusCode.NotFound));

    var result = await client.GetDataAgentByCodeAsync("NEEXISTUJICI-AGENT", acceptNotFound: true, CancellationToken.None);

    result.ShouldBeNull();
  }

  /// <summary>
  /// URL pro GetDataAgentByCodeAsync musí obsahovat URL-zakódovaný kód agenta.
  /// </summary>
  [Fact]
  public async Task GetDataAgentByCodeAsync_SpecialCharactersInCode_UrlEncodesCode()
  {
    var handler = new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.NotFound));
    var client = CreateClient(handler);
    var code = "Agent/With Spaces&Special=Chars";

    await client.GetDataAgentByCodeAsync(code, acceptNotFound: true, CancellationToken.None);

    var requestUri = handler.LastRequest?.RequestUri?.ToString();
    requestUri.ShouldNotBeNull();
    // Lomítko a speciální znaky musí být zakódovány; mezery může .NET normalizovat (%20 ↔ space)
    requestUri.ShouldContain("Agent%2FWith"); // '/' → %2F
    requestUri.ShouldContain("Special%3DChars"); // '=' → %3D
  }

  // ---------------------------------------------------------------------------
  // SwitchEnabledDataAgentAsync
  // ---------------------------------------------------------------------------

  /// <summary>
  /// Prázdné ID data-agenta musí způsobit ArgumentNullException.
  /// </summary>
  [Fact]
  public async Task SwitchEnabledDataAgentAsync_EmptyId_ThrowsArgumentNullException()
  {
    var client = CreateClient(new HttpResponseMessage(HttpStatusCode.OK));

    await Should.ThrowAsync<ArgumentNullException>(() =>
      client.SwitchEnabledDataAgentAsync(string.Empty, enabled: true, CancellationToken.None));
  }

  /// <summary>
  /// HTTP 404 — data-agent neexistuje, vrátí <c>false</c>.
  /// </summary>
  [Fact]
  public async Task SwitchEnabledDataAgentAsync_NotFound_ReturnsFalse()
  {
    var client = CreateClient(new HttpResponseMessage(HttpStatusCode.NotFound));

    var result = await client.SwitchEnabledDataAgentAsync("neexistujici-id", enabled: true, CancellationToken.None);

    result.ShouldBeFalse();
  }

  /// <summary>
  /// HTTP 200 — přepnutí proběhlo úspěšně, vrátí <c>true</c>.
  /// </summary>
  [Fact]
  public async Task SwitchEnabledDataAgentAsync_Success_ReturnsTrue()
  {
    var client = CreateClient(new HttpResponseMessage(HttpStatusCode.OK));

    var result = await client.SwitchEnabledDataAgentAsync("existing-agent-id", enabled: true, CancellationToken.None);

    result.ShouldBeTrue();
  }

  /// <summary>
  /// HTTP 204 No Content — přepnutí proběhlo úspěšně, vrátí <c>true</c>.
  /// </summary>
  [Fact]
  public async Task SwitchEnabledDataAgentAsync_NoContent_ReturnsTrue()
  {
    var client = CreateClient(new HttpResponseMessage(HttpStatusCode.NoContent));

    var result = await client.SwitchEnabledDataAgentAsync("existing-agent-id", enabled: false, CancellationToken.None);

    result.ShouldBeTrue();
  }

  // ---------------------------------------------------------------------------
  // SwitchEnabledDataSourceAsync
  // ---------------------------------------------------------------------------

  /// <summary>
  /// Prázdné ID datového zdroje musí způsobit ArgumentNullException.
  /// </summary>
  [Fact]
  public async Task SwitchEnabledDataSourceAsync_EmptyId_ThrowsArgumentNullException()
  {
    var client = CreateClient(new HttpResponseMessage(HttpStatusCode.OK));

    await Should.ThrowAsync<ArgumentNullException>(() =>
      client.SwitchEnabledDataSourceAsync(string.Empty, enabled: true, CancellationToken.None));
  }

  /// <summary>
  /// HTTP 404 — datový zdroj neexistuje, vrátí <c>false</c>.
  /// </summary>
  [Fact]
  public async Task SwitchEnabledDataSourceAsync_NotFound_ReturnsFalse()
  {
    var client = CreateClient(new HttpResponseMessage(HttpStatusCode.NotFound));

    var result = await client.SwitchEnabledDataSourceAsync("neexistujici-zdroj", enabled: true, CancellationToken.None);

    result.ShouldBeFalse();
  }

  /// <summary>
  /// HTTP 200 — přepnutí proběhlo úspěšně, vrátí <c>true</c>.
  /// </summary>
  [Fact]
  public async Task SwitchEnabledDataSourceAsync_Success_ReturnsTrue()
  {
    var client = CreateClient(new HttpResponseMessage(HttpStatusCode.OK));

    var result = await client.SwitchEnabledDataSourceAsync("existing-source-id", enabled: false, CancellationToken.None);

    result.ShouldBeTrue();
  }

  /// <summary>
  /// URL pro SwitchEnabledDataSourceAsync musí obsahovat ID zdroje a segment SwitchEnabled.
  /// </summary>
  [Fact]
  public async Task SwitchEnabledDataSourceAsync_BuildsCorrectUrl()
  {
    var handler = new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK));
    var client = CreateClient(handler);
    var sourceId = "test-source-123";

    await client.SwitchEnabledDataSourceAsync(sourceId, enabled: true, CancellationToken.None);

    var requestUri = handler.LastRequest?.RequestUri?.ToString();
    requestUri.ShouldNotBeNull();
    requestUri.ShouldContain(sourceId);
    requestUri.ShouldContain("SwitchEnabled");
  }

  // ---------------------------------------------------------------------------
  // Pomocné třídy
  // ---------------------------------------------------------------------------

  private static ICustomDataServiceClient CreateClient(HttpResponseMessage response)
    => CreateClient(new FakeHttpMessageHandler(response));

  private static ICustomDataServiceClient CreateClient(FakeHttpMessageHandler handler)
  {
    var httpClient = new HttpClient(handler)
    {
      BaseAddress = new Uri("http://avaplace-test.local")
    };

    var options = DataServiceClientOptions.Default;
    options.BaseUrl = "http://avaplace-test.local";

    return new TestableCustomDataServiceClient(Options.Create(options), httpClient);
  }

  // ---------------------------------------------------------------------------

  /// <summary>
  /// Testovatelná podtřída zpřístupňující protected konstruktor.
  /// </summary>
  private class TestableCustomDataServiceClient : CustomDataServiceClient
  {
    public TestableCustomDataServiceClient(
      IOptions<DataServiceClientOptions> options,
      HttpClient httpClient)
      : base(options.Value, NullLogger<CustomDataServiceClient>.Instance,
             new FakeTokenProvider(), [], httpClient, manageBaseClient: false)
    {
    }
  }

  /// <summary>
  /// Fake HTTP handler zachycující volání a vracející přednastavený response.
  /// </summary>
  private class FakeHttpMessageHandler : HttpMessageHandler
  {
    private readonly HttpResponseMessage _response;

    /// <summary>Poslední zachycený request (pro ověření URL).</summary>
    public HttpRequestMessage? LastRequest { get; private set; }

    public FakeHttpMessageHandler(HttpResponseMessage response)
    {
      _response = response;
    }

    protected override Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request, CancellationToken cancellationToken)
    {
      LastRequest = request;
      return Task.FromResult(_response);
    }
  }

  /// <summary>
  /// Fake implementace <see cref="IConnectorTokenProvider"/> vracející testovací Bearer token.
  /// </summary>
  private class FakeTokenProvider : IConnectorTokenProvider
  {
    public Task<AuthenticationHeaderValue> GetAuthenticationHeaderAsync(IApiClient apiClient, CancellationToken ct = default)
      => Task.FromResult(new AuthenticationHeaderValue("Bearer", "fake-test-token"));
  }
}

// ---------------------------------------------------------------------------
// Live integrační testy — volání skutečného AVAPlace demo prostředí
// ---------------------------------------------------------------------------

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

}
