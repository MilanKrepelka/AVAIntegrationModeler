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
using AVAIntegrationModeler.Contracts.Scenarios;

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
      _logger.LogInformation($"{nameof(GetDataModels)} cancelled by token. datasource={datasource}");
      throw;
    }
    catch (HttpRequestException ex)
    {
      _logger.LogError(ex, $"HTTP error in {nameof(GetDataModels)}. datasource={datasource}");
      throw;
    }
    catch (JsonException ex)
    {
      _logger.LogError(ex, $"JSON deserialization error in {nameof(GetDataModels)}. datasource={datasource}");
      throw;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Unexpected error in {nameof(GetDataModels)}. datasource={datasource}");
      throw;
    }
  }

  /// <inheritdoc/>
  public async Task<ScenarioDTO> GetScenario(Datasource datasource, Guid scenarioId, CancellationToken cancellationToken)
  {
    if (!Enum.IsDefined(typeof(Datasource), datasource))
      throw new ArgumentOutOfRangeException(nameof(datasource));

    try
    {
      _logger.LogDebug($"GetDataModels starting. datasource={datasource}");

      var fluent = new FluentClient(_httpClient);

      // Execute with FluentClient and get deserialized result (keep using FluentClient)
      var response = await fluent
        .GetAsync($"scenarios/{datasource}/{scenarioId}")
        //.WithArgument("datasource", datasource)
        //.WithArgument("scenarioId", scenarioId)
        .WithCancellationToken(cancellationToken)
        .As<ScenarioDTO>();

      return response ?? new ScenarioDTO();
    }
    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
    {
      _logger.LogInformation($"{nameof(GetScenario)} cancelled by token. datasource={datasource} scenarioId={scenarioId}");
      throw;
    }
    catch (HttpRequestException ex)
    {
      _logger.LogError(ex, $"HTTP error in {nameof(GetScenario)}. datasource={datasource} scenarioId={scenarioId}");
      throw;
    }
    catch (JsonException ex)
    {
      _logger.LogError(ex, $"JSON deserialization error in {nameof(GetScenario)}. datasource={datasource} scenarioId={scenarioId}");
      throw;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Unexpected error in {nameof(GetScenario)}. datasource={datasource} scenarioId={scenarioId}");
      throw;
    }
  }

  /// <inheritdoc/>
  public async Task<ScenarioListResponse> GetScenarios(Datasource datasource, CancellationToken cancellationToken)
  {
    if (!Enum.IsDefined(typeof(Datasource), datasource))
      throw new ArgumentOutOfRangeException(nameof(datasource));

    try
    {
      _logger.LogDebug($"{nameof(GetScenarios)} starting. datasource={datasource}");

      var fluent = new FluentClient(_httpClient);

      // Execute with FluentClient and get deserialized result (keep using FluentClient)
      var response = await fluent
        .GetAsync("scenarios")
        .WithArgument("datasource", datasource)
        .WithCancellationToken(cancellationToken)
        .As<ScenarioListResponse>();

      var count = response?.Scenarios?.Count ?? 0;
      _logger.LogInformation($"{nameof(GetScenarios)} completed. datasource={datasource} returned {count} items");

      return response ?? new ScenarioListResponse();
    }
    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
    {
      _logger.LogInformation($"{nameof(GetScenarios)} cancelled by token. datasource={datasource}");
      throw;
    }
    catch (HttpRequestException ex)
    {
      _logger.LogError(ex, $"HTTP error in {nameof(GetScenarios)}. datasource={datasource}");
      throw;
    }
    catch (JsonException ex)
    {
      _logger.LogError(ex, $"JSON deserialization error in {nameof(GetScenarios)}. datasource={datasource}");
      throw;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Unexpected error in {nameof(GetScenarios)}. datasource={datasource}");
      throw;
    }
  }
}
