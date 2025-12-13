using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using Pathoschild.Http.Client;
using Microsoft.Extensions.Logging;

namespace AVAIntegrationModeler.API.Client;


/// <inheritdoc/>
public class AVAIntegrationModelerApiClient : IAVAIntegrationModelerApiClient
{
  private readonly HttpClient _httpClient;
  private readonly ILogger<AVAIntegrationModelerApiClient> _logger;

  /// <inheritdoc/>
  public AVAIntegrationModelerApiClient(HttpClient httpClient, ILogger<AVAIntegrationModelerApiClient> logger)
  {
    _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  /// <inheritdoc/>
  public async Task<DataModelListResponse> GetDataModels(Datasource datasource, CancellationToken cancellationToken)
  {
    if (!Enum.IsDefined(typeof(Datasource), datasource))
      throw new ArgumentOutOfRangeException(nameof(datasource));

    try
    {
      _logger.LogDebug($"GetDataModels starting. datasource={datasource}");

      var fluent = new FluentClient(_httpClient);

      // Execute with FluentClient and get deserialized result (keep using FluentClient)
      var response = await fluent
        .GetAsync("datamodels")
        .WithArgument("datasource", datasource)
        .WithCancellationToken(cancellationToken)
        .As<DataModelListResponse>();

      var count = response?.DataModels?.Count ?? 0;
      _logger.LogInformation($"GetDataModels completed. datasource={datasource} returned {count} items");

      return response ?? new DataModelListResponse();
    }
    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
    {
      _logger.LogInformation($"GetDataModels cancelled by token. datasource={datasource}");
      throw;
    }
    catch (HttpRequestException ex)
    {
      _logger.LogError(ex, $"HTTP error in GetDataModels. datasource={datasource}");
      throw;
    }
    catch (JsonException ex)
    {
      _logger.LogError(ex, $"JSON deserialization error in GetDataModels. datasource={datasource}");
      throw;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Unexpected error in GetDataModels. datasource={datasource}");
      throw;
    }
  }
}
