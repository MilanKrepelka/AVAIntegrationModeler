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
  // CreateMetadataVersionAsync
  // ---------------------------------------------------------------------------

  /// <summary>
  /// Správná odpověď se JSON polem „code" musí vrátit kód verze.
  /// </summary>
  [Fact]
  public async Task CreateMetadataVersionAsync_Success_ReturnsVersionCode()
  {
    var response = JsonResponse(HttpStatusCode.OK, """{"code":"v-2026-1"}""");
    var client = CreateClient(response);

    var result = await client.CreateMetadataVersionAsync(CancellationToken.None);

    result.ShouldBe("v-2026-1");
  }

  /// <summary>
  /// Pokud odpověď neobsahuje pole „code", musí být vyhozena <see cref="InvalidOperationException"/>.
  /// </summary>
  [Fact]
  public async Task CreateMetadataVersionAsync_MissingCode_ThrowsInvalidOperationException()
  {
    var response = JsonResponse(HttpStatusCode.OK, """{"result":"ok"}""");
    var client = CreateClient(response);

    await Should.ThrowAsync<InvalidOperationException>(() =>
      client.CreateMetadataVersionAsync(CancellationToken.None));
  }

  /// <summary>
  /// URL musí cílit na segment <c>Process/CreateMetadataVersion</c> a použít metodu POST.
  /// </summary>
  [Fact]
  public async Task CreateMetadataVersionAsync_CallsCorrectUrl()
  {
    var handler = new FakeHttpMessageHandler(JsonResponse(HttpStatusCode.OK, """{"code":"v1"}"""));
    var client = CreateClient(handler);

    await client.CreateMetadataVersionAsync(CancellationToken.None);

    handler.LastRequest.ShouldNotBeNull();
    handler.LastRequest!.Method.ShouldBe(HttpMethod.Post);
    handler.LastRequest.RequestUri!.ToString().ShouldContain("Process/CreateMetadataVersion");
  }

  // ---------------------------------------------------------------------------
  // ImportDataModelAsync
  // ---------------------------------------------------------------------------

  /// <summary>
  /// Prázdná verze musí způsobit <see cref="ArgumentNullException"/> před HTTP voláním.
  /// </summary>
  [Fact]
  public async Task ImportDataModelAsync_EmptyTargetVersion_ThrowsArgumentNullException()
  {
    var client = CreateClient(new HttpResponseMessage(HttpStatusCode.OK));
    using var content = new MemoryStream(Encoding.UTF8.GetBytes("{}"));

    await Should.ThrowAsync<ArgumentNullException>(() =>
      client.ImportDataModelAsync(string.Empty, allowUpdate: true, content, CancellationToken.None));
  }

  /// <summary>
  /// Null stream musí způsobit <see cref="ArgumentNullException"/> před HTTP voláním.
  /// </summary>
  [Fact]
  public async Task ImportDataModelAsync_NullStream_ThrowsArgumentNullException()
  {
    var client = CreateClient(new HttpResponseMessage(HttpStatusCode.OK));

    await Should.ThrowAsync<ArgumentNullException>(() =>
      client.ImportDataModelAsync("v1", allowUpdate: true, null!, CancellationToken.None));
  }

  /// <summary>
  /// HTTP 200 — import proběhl úspěšně, žádná výjimka.
  /// </summary>
  [Fact]
  public async Task ImportDataModelAsync_Success_DoesNotThrow()
  {
    var client = CreateClient(new HttpResponseMessage(HttpStatusCode.OK));
    using var content = new MemoryStream(Encoding.UTF8.GetBytes("""{"code":"dm-Test"}"""));

    await Should.NotThrowAsync(() =>
      client.ImportDataModelAsync("v-2026-1", allowUpdate: true, content, CancellationToken.None));
  }

  /// <summary>
  /// HTTP 500 musí způsobit výjimku (přes <c>EnsureSuccessStatusCode</c>).
  /// </summary>
  [Fact]
  public async Task ImportDataModelAsync_ServerError_Throws()
  {
    var client = CreateClient(new HttpResponseMessage(HttpStatusCode.InternalServerError));
    using var content = new MemoryStream(Encoding.UTF8.GetBytes("{}"));

    await Should.ThrowAsync<HttpRequestException>(() =>
      client.ImportDataModelAsync("v1", allowUpdate: false, content, CancellationToken.None));
  }

  /// <summary>
  /// URL musí obsahovat <c>importdatamodel</c>, query param <c>targetVersion</c> a <c>allowupdate</c>.
  /// Request musí použít metodu POST.
  /// </summary>
  [Fact]
  public async Task ImportDataModelAsync_BuildsCorrectUrl()
  {
    var handler = new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK));
    var client = CreateClient(handler);
    using var content = new MemoryStream(Encoding.UTF8.GetBytes("{}"));

    await client.ImportDataModelAsync("ver-42", allowUpdate: true, content, CancellationToken.None);

    handler.LastRequest.ShouldNotBeNull();
    handler.LastRequest!.Method.ShouldBe(HttpMethod.Post);
    var uri = handler.LastRequest.RequestUri!.ToString();
    uri.ShouldContain("importdatamodel");
    uri.ShouldContain("targetVersion=ver-42");
    uri.ShouldContain("allowupdate=True");
  }

  /// <summary>
  /// Body requestu musí mít Content-Type <c>application/json</c>.
  /// </summary>
  [Fact]
  public async Task ImportDataModelAsync_SetsJsonContentType()
  {
    var handler = new FakeHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK));
    var client = CreateClient(handler);
    using var content = new MemoryStream(Encoding.UTF8.GetBytes("""{"code":"dm-X"}"""));

    await client.ImportDataModelAsync("v1", allowUpdate: false, content, CancellationToken.None);

    handler.LastRequest.ShouldNotBeNull();
    handler.LastRequest!.Content.ShouldNotBeNull();
    handler.LastRequest.Content!.Headers.ContentType?.MediaType.ShouldBe("application/json");
  }

  // ---------------------------------------------------------------------------
  // Pomocné třídy
  // ---------------------------------------------------------------------------

  private static HttpResponseMessage JsonResponse(HttpStatusCode status, string json)
  {
    var response = new HttpResponseMessage(status);
    response.Content = new StringContent(json, Encoding.UTF8, "application/json");
    return response;
  }

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
