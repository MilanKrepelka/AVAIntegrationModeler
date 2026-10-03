using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using ASOL.Core.ApiConnector;
using ASOL.Core.Localization;
using ASOL.DataService.Connector;
using ASOL.DataService.Connector.Options;
using ASOL.DataService.Contracts;
using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Pathoschild.Http.Client;

namespace AVAIntegrationModeler.AVAPlace.API.Connectors;

/// <inheritdoc cref="ICustomDataServiceClient"/>
public class CustomDataServiceClient : DataServiceClient, ICustomDataServiceClient
{
  protected override string ApiVersionPrefix => "api/v1";

  /// <summary>
  /// Initializes the instance.
  /// </summary>
  /// <param name="options">Client options</param>
  /// <param name="logger">Logger</param>
  /// <param name="tokenProvider">Token provider</param>
  /// <param name="requestHeaderProviders">Request headers providers</param>
  public CustomDataServiceClient(
      IOptions<DataServiceClientOptions> options,
      ILogger<CustomDataServiceClient> logger,
      IConnectorTokenProvider tokenProvider,
      IEnumerable<IRequestHeaderProvider> requestHeaderProviders)
      : this(options?.Value!, logger, tokenProvider, requestHeaderProviders, baseClient: null)
  {
  }

  /// .ctor
  protected CustomDataServiceClient(
      DataServiceClientOptions options,
      ILogger<CustomDataServiceClient> logger,
      IConnectorTokenProvider tokenProvider,
      IEnumerable<IRequestHeaderProvider> requestHeaderProviders,
      HttpClient? baseClient = null,
      bool manageBaseClient = false)
      : base(options, logger, tokenProvider, requestHeaderProviders, baseClient, manageBaseClient)
  {
  }

  /// <inheritdoc/>
  public async Task<DataAgentDefinition?> GetDataAgentByCodeAsync(string agentCode, bool acceptNotFound = false, CancellationToken ct = default)
  {
    if (string.IsNullOrEmpty(agentCode)) throw new ArgumentNullException(nameof(agentCode));
    ct.ThrowIfCancellationRequested();

    var resource = $"{ApiVersionPrefix}/DataAgents/{Uri.EscapeDataString(agentCode)}";
    Logger.LogDebug($"Retrieving data-agent by code from {CombineUri(resource)}");

    var request = (await AddAuthentication(Client.GetAsync(resource), ct))
        .WithCancellationToken(ct);

    var result = await GetSingleResultAsync<DataAgentDefinition>(request, acceptNotFound, ct);
    return result;
  }

  /// <inheritdoc/>
  public async Task<bool> SwitchEnabledDataAgentAsync(string dataAgentId, bool enabled, CancellationToken ct = default)
  {
    if (string.IsNullOrEmpty(dataAgentId)) throw new ArgumentNullException(nameof(dataAgentId));
    ct.ThrowIfCancellationRequested();

    var resource = $"{ApiVersionPrefix}/DataAgents/{Uri.EscapeDataString(dataAgentId)}/SwitchEnabled";
    Logger.LogDebug($"Switching enabled for data-agent on {CombineUri(resource)}");

    var request = (await AddAuthentication(Client.PostAsync(resource), ct))
        .WithArgument(nameof(enabled), enabled)
        .WithOptions(ignoreHttpErrors: true)
        .WithCancellationToken(ct);

    var result = await request.AsMessage();
    switch (result.StatusCode)
    {
      case HttpStatusCode.NotFound:
        return false;

      default:
        result.EnsureSuccessStatusCode();
        return true;
    }
  }

  /// <inheritdoc/>
  public async Task<bool> SwitchEnabledDataSourceAsync(string dataSourceId, bool enabled, CancellationToken ct = default)
  {
    if (string.IsNullOrEmpty(dataSourceId)) throw new ArgumentNullException(nameof(dataSourceId));
    ct.ThrowIfCancellationRequested();

    var resource = $"{ApiVersionPrefix}/DataSources/{Uri.EscapeDataString(dataSourceId)}/SwitchEnabled";
    Logger.LogDebug($"Switching enabled for data-source on {CombineUri(resource)}");

    var request = (await AddAuthentication(Client.PostAsync(resource), ct))
        .WithArgument(nameof(enabled), enabled)
        .WithOptions(ignoreHttpErrors: true)
        .WithCancellationToken(ct);

    var result = await request.AsMessage();
    switch (result.StatusCode)
    {
      case HttpStatusCode.NotFound:
        return false;

      default:
        result.EnsureSuccessStatusCode();
        return true;
    }
  }

  /// <summary>
  /// Vrátí unifikovaná data pro daný model.
  /// </summary>
  /// <param name="ModelId">Identifikátor modelu</param>
  /// <param name="ct">Token pro zrušení operace</param>
  /// <returns>Seznam unifikovaných datových objektů</returns>
  public async Task<IList<Models.DataModelRecord>> GetUnifiedDataAsync(Guid ModelId, CancellationToken ct = default)
  {
    var resource = $"{ApiVersionPrefix}/Process/GetUnifiedData/{ModelId}";
    Logger.LogDebug($"Retrieving unified data for model on {CombineUri(resource)}");

    var response = await (await AddAuthentication(Client.GetAsync(resource), ct))
        .WithOptions(ignoreHttpErrors: true)
        .WithCancellationToken(ct);

    if (response.Status == System.Net.HttpStatusCode.NotFound)
    {
      Logger.LogWarning($"Model {ModelId} not found, returning empty list");
      return new List<Models.DataModelRecord>();
    }

    // Parsujeme přes JObject aby nám nevadil type mismatch (např. Description jako string vs LocalizedValue<string>).
    var root = await response.As<JObject>();
    var items = root?["items"] as Newtonsoft.Json.Linq.JArray ?? root?["Items"] as Newtonsoft.Json.Linq.JArray;
    if (items is null) return new List<Models.DataModelRecord>();

    var result = new List<Models.DataModelRecord>();
    foreach (var item in items)
    {
      var record = new Models.DataModelRecord
      {
        Id = new Models.DataModelRecordCompositeId
        {
          ModelId = item["Id"]?["ModelId"]?.ToObject<Guid>() ?? Guid.Empty,
          RecordId = item["Id"]?["RecordId"]?.ToObject<Guid>() ?? Guid.Empty,
        },
        ExternalId = item["ExternalId"]?.ToString(),
        SourceId = item["SourceId"]?.ToObject<Guid>() ?? Guid.Empty,
        MandantCode = item["MandantCode"]?.ToString(),
        Released = item["Released"]?.ToObject<bool>() ?? false,
        UtcCreatedOn = item["UtcCreatedOn"]?.ToObject<DateTimeOffset>() ?? default,
        UtcModifiedOn = item["UtcModifiedOn"]?.ToObject<DateTimeOffset>() ?? default,
        Code = item["Code"]?.ToString(),
        Name = ParseLocalizedValue(item["Name"]),
        Description = ParseLocalizedValue(item["Description"]),
      };
      result.Add(record);
    }
    return result;
  }

  /// <summary>
  /// Parsuje JSON token na <see cref="LocalizedValue{T}"/>. Podporuje objekt se 'values' polem i plain string.
  /// </summary>
  private static LocalizedValue<string>? ParseLocalizedValue(JToken? token)
  {
    if (token == null || token.Type == JTokenType.Null)
      return null;

    if (token.Type == JTokenType.Object)
      return token.ToObject<LocalizedValue<string>>();

    if (token.Type == JTokenType.String)
    {
      var str = token.ToString();
      return new LocalizedValue<string>
      {
        Values = new List<LocalizedValueItem<string>>
        {
          new() { Locale = "cs-CZ", Value = str },
          new() { Locale = "en-US", Value = str },
        }
      };
    }

    return null;
  }

  /// <inheritdoc/>
  public async Task<string> CreateMetadataVersionAsync(CancellationToken ct = default)
  {
    var resource = $"{ApiVersionPrefix}/Process/CreateMetadataVersion";
    Logger.LogDebug($"Creating metadata version on {CombineUri(resource)}");

    var request = (await AddAuthentication(Client.PostAsync(resource), ct))
        .WithOptions(ignoreHttpErrors: true)
        .WithCancellationToken(ct);

    var response = await request.AsMessage();
    if (!response.IsSuccessStatusCode)
    {
      var responseBody = await response.Content.ReadAsStringAsync(ct);
      Logger.LogError($"CreateMetadataVersion selhalo: {(int)response.StatusCode} {response.StatusCode} — {responseBody}");
      throw new HttpRequestException(
          $"CreateMetadataVersion selhalo: {(int)response.StatusCode} {response.StatusCode} — {responseBody}",
          inner: null,
          statusCode: response.StatusCode);
    }

    var resultJson = await response.Content.ReadAsStringAsync(ct);
    var result = JObject.Parse(resultJson);
    var code = result?["code"]?.ToString();
    if (string.IsNullOrEmpty(code))
      throw new InvalidOperationException("DataService nevrátil kód nově vytvořené verze metadat.");

    return code;
  }

  /// <inheritdoc/>
  public async Task ImportDataModelAsync(string targetVersion, bool allowUpdate, Stream jsonContent, CancellationToken ct = default)
  {
    if (string.IsNullOrEmpty(targetVersion)) throw new ArgumentNullException(nameof(targetVersion));
    if (jsonContent is null) throw new ArgumentNullException(nameof(jsonContent));
    ct.ThrowIfCancellationRequested();

    var resource = $"{ApiVersionPrefix}/process/importdatamodel";
    Logger.LogDebug($"Importing data model to {CombineUri(resource)}, targetVersion={targetVersion}, allowUpdate={allowUpdate}");

    if (Logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))
    {
      var pos = jsonContent.Position;
      using var reader = new System.IO.StreamReader(jsonContent, System.Text.Encoding.UTF8, leaveOpen: true);
      var json = await reader.ReadToEndAsync(ct);
      Logger.LogDebug($"ImportDataModel payload: {json}");
      jsonContent.Position = pos;
    }

    var body = new StreamContent(jsonContent);
    body.Headers.ContentType = new MediaTypeHeaderValue("application/json") { CharSet = "utf-8" };

    var request = (await AddAuthentication(Client.PostAsync(resource), ct))
        .WithArgument("targetVersion", targetVersion)
        .WithArgument("allowupdate", allowUpdate)
        .WithBody(body)
        .WithOptions(ignoreHttpErrors: true)
        .WithCancellationToken(ct);

    var response = await request.AsMessage();
    if (!response.IsSuccessStatusCode)
    {
      var responseBody = await response.Content.ReadAsStringAsync(ct);
      Logger.LogError($"ImportDataModel selhalo: {(int)response.StatusCode} {response.StatusCode} — {responseBody}");
      throw new HttpRequestException(
          $"ImportDataModel selhalo pro verzi {targetVersion}: {(int)response.StatusCode} {response.StatusCode} — {responseBody}",
          inner: null,
          statusCode: response.StatusCode);
    }
  }

  /// <inheritdoc/>
  public async Task<string> GetDataModelMarkdownDocumentAsync(Guid modelId, int months = 12, CancellationToken ct = default)
  {
    ct.ThrowIfCancellationRequested();

    var resource = $"{ApiVersionPrefix}/Process/MarkdownDocument/{modelId}";
    Logger.LogDebug($"Retrieving data model markdown document from {CombineUri(resource)}, months={months}");

    var response = await (await AddAuthentication(Client.GetAsync(resource), ct))
        .WithArgument("months", months)
        .WithOptions(ignoreHttpErrors: true)
        .WithCancellationToken(ct);

    if (!response.Status.Equals(System.Net.HttpStatusCode.OK))
    {
      var responseBody = await response.Message.Content.ReadAsStringAsync(ct);
      Logger.LogError($"GetDataModelMarkdownDocument selhalo: {(int)response.Status} {response.Status} — {responseBody}");
      throw new HttpRequestException(
          $"GetDataModelMarkdownDocument selhalo: {(int)response.Status} {response.Status} — {responseBody}",
          inner: null,
          statusCode: response.Status);
    }

    return await response.Message.Content.ReadAsStringAsync(ct);
  }

  /// <inheritdoc/>
  public async Task<string> GetMonthlyMetadataDifferencesDocumentAsync(int months = 12, CancellationToken ct = default)
  {
    ct.ThrowIfCancellationRequested();

    var resource = $"{ApiVersionPrefix}/Process/MonthlyMetadataDifferencesDocument";
    Logger.LogDebug($"Retrieving monthly metadata differences document from {CombineUri(resource)}, months={months}");

    var response = await (await AddAuthentication(Client.GetAsync(resource), ct))
        .WithArgument("months", months)
        .WithOptions(ignoreHttpErrors: true)
        .WithCancellationToken(ct);

    if (!response.Status.Equals(System.Net.HttpStatusCode.OK))
    {
      var responseBody = await response.Message.Content.ReadAsStringAsync(ct);
      Logger.LogError($"GetMonthlyMetadataDifferencesDocument selhalo: {(int)response.Status} {response.Status} — {responseBody}");
      throw new HttpRequestException(
          $"GetMonthlyMetadataDifferencesDocument selhalo: {(int)response.Status} {response.Status} — {responseBody}",
          inner: null,
          statusCode: response.Status);
    }

    return await response.Message.Content.ReadAsStringAsync(ct);
  }

  /// <inheritdoc/>
  public async Task ImportUnifiedDataAsync(string modelCode, string targetVersion, bool allowUpdate, bool allowChangeExternalId, Stream jsonContent, CancellationToken ct = default)
  {
    if (string.IsNullOrEmpty(modelCode)) throw new ArgumentNullException(nameof(modelCode));
    if (string.IsNullOrEmpty(targetVersion)) throw new ArgumentNullException(nameof(targetVersion));
    if (jsonContent is null) throw new ArgumentNullException(nameof(jsonContent));
    ct.ThrowIfCancellationRequested();

    var resource = $"{ApiVersionPrefix}/process/importunifieddata/{Uri.EscapeDataString(modelCode)}";
    Logger.LogDebug($"Importing unified data to {CombineUri(resource)}, targetVersion={targetVersion}, allowUpdate={allowUpdate}, allowChangeExternalId={allowChangeExternalId}");

    if (Logger.IsEnabled(Microsoft.Extensions.Logging.LogLevel.Debug))
    {
      var pos = jsonContent.Position;
      using var reader = new System.IO.StreamReader(jsonContent, System.Text.Encoding.UTF8, leaveOpen: true);
      var json = await reader.ReadToEndAsync(ct);
      Logger.LogDebug($"ImportUnifiedData payload ({modelCode}): {json}");
      jsonContent.Position = pos;
    }

    var body = new StreamContent(jsonContent);
    body.Headers.ContentType = new MediaTypeHeaderValue("application/json") { CharSet = "utf-8" };

    var request = (await AddAuthentication(Client.PostAsync(resource), ct))
        .WithArgument("targetVersion", targetVersion)
        .WithArgument("allowupdate", allowUpdate)
        .WithArgument("allowChangeExternalId", allowChangeExternalId)
        .WithBody(body)
        .WithOptions(ignoreHttpErrors: true)
        .WithCancellationToken(ct);

    var response = await request.AsMessage();
    if (!response.IsSuccessStatusCode)
    {
      var responseBody = await response.Content.ReadAsStringAsync(ct);
      Logger.LogError($"ImportUnifiedData selhalo: {(int)response.StatusCode} {response.StatusCode} — {responseBody}");
      throw new HttpRequestException(
          $"ImportUnifiedData selhalo pro model {modelCode}, verzi {targetVersion}: {(int)response.StatusCode} {response.StatusCode} — {responseBody}",
          inner: null,
          statusCode: response.StatusCode);
    }
  }
}

