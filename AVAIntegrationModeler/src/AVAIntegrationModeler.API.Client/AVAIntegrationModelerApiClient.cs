using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Contracts.Scenarios;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pathoschild.Http.Client;

    namespace AVAIntegrationModeler.API.Client;

    public class AVAIntegrationModelerApiClient : IAVAIntegrationModelerApiClient
{
  private readonly HttpClient _httpClient;
  private readonly ILogger<AVAIntegrationModelerApiClient> _logger;

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
  public async Task<ScenarioDTO> GetScenario(Datasource datasource, string scenarioCode, CancellationToken cancellationToken)
  {
    if (!Enum.IsDefined(typeof(Datasource), datasource))
      throw new ArgumentOutOfRangeException(nameof(datasource));

    try
    {
      _logger.LogDebug($"GetDataModels starting. datasource={datasource}");

      var fluent = new FluentClient(_httpClient);

      // Execute with FluentClient and get deserialized result (keep using FluentClient)
      var response = await fluent
        .GetAsync($"scenarios/{datasource}/{scenarioCode}")
        //.WithArgument("datasource", datasource)
        //.WithArgument("scenarioId", scenarioId)
        .WithCancellationToken(cancellationToken)
        .As<ScenarioDTO>();

      return response ?? new ScenarioDTO();
    }
    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
    {
      _logger.LogInformation($"{nameof(GetScenario)} cancelled by token. datasource={datasource} scenarioCode={scenarioCode}");
      throw;
    }
    catch (HttpRequestException ex)
    {
      _logger.LogError(ex, $"HTTP error in {nameof(GetScenario)}. datasource={datasource} scenarioCode={scenarioCode}");
      throw;
    }
    catch (JsonException ex)
    {
      _logger.LogError(ex, $"JSON deserialization error in {nameof(GetScenario)}. datasource={datasource} scenarioCode={scenarioCode}");
      throw;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Unexpected error in {nameof(GetScenario)}. datasource={datasource} scenarioCode={scenarioCode}");
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

  /// <inheritdoc/>
  public async Task<FeatureListResponse> GetFeatures(Datasource datasource, CancellationToken cancellationToken)
  {
    if (!Enum.IsDefined(typeof(Datasource), datasource))
      throw new ArgumentOutOfRangeException(nameof(datasource));

    try
    {
      _logger.LogDebug($"{nameof(GetFeatures)} starting. datasource={datasource}");

      var fluent = new FluentClient(_httpClient);

      // Execute with FluentClient and get deserialized result (keep using FluentClient)
      var response = await fluent
        .GetAsync("features")
        .WithArgument("datasource", datasource)
        .WithCancellationToken(cancellationToken)
        .As<FeatureListResponse>();

      var count = response?.Features?.Count ?? 0;
      _logger.LogInformation($"{nameof(GetFeatures)} completed. datasource={datasource} returned {count} items");

      return response ?? new FeatureListResponse();
    }
    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
    {
      _logger.LogInformation($"{nameof(GetFeatures)} cancelled by token. datasource={datasource}");
      throw;
    }
    catch (HttpRequestException ex)
    {
      _logger.LogError(ex, $"HTTP error in {nameof(GetFeatures)}. datasource={datasource}");
      throw;
    }
    catch (JsonException ex)
    {
      _logger.LogError(ex, $"JSON deserialization error in {nameof(GetFeatures)}. datasource={datasource}");
      throw;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Unexpected error in {nameof(GetFeatures)}. datasource={datasource}");
      throw;
    }
  }

  /// <inheritdoc/>
  public async Task<Result<Guid>> CreateScenario(Datasource datasource, ScenarioDTO scenario, CancellationToken cancellationToken)
  {
    if (!Enum.IsDefined(typeof(Datasource), datasource))
      return Result<Guid>.Invalid(new ValidationError(nameof(datasource), "Neplatný datasource."));
    
    if (scenario is null)
      return Result<Guid>.Invalid(new ValidationError(nameof(scenario), "Scenario nesmí být null."));

    _logger.LogDebug($"{nameof(CreateScenario)} starting. datasource={datasource}, scenarioCode={scenario.Code}");

    var fluent = new FluentClient(_httpClient);

    var result = await fluent
      .PostAsync("scenarios")
      .WithArgument("datasource", datasource)
      .WithBody(scenario)
      .WithCancellationToken(cancellationToken)
      .AsResult<Guid>(); // ← Použití nové extension metody

    if (result.IsSuccess)
      _logger.LogInformation($"{nameof(CreateScenario)} completed. datasource={datasource}, scenarioId={result.Value}");
    else
      _logger.LogWarning($"{nameof(CreateScenario)} failed. datasource={datasource}, status={result.Status}");

    return result;
  }

  /// <inheritdoc/>
  public async Task<ScenarioDTO> UpdateScenario(Datasource datasource, ScenarioDTO scenario, CancellationToken cancellationToken)
  {
    if (!Enum.IsDefined(typeof(Datasource), datasource))
      throw new ArgumentOutOfRangeException(nameof(datasource));
    if (scenario is null)
      throw new ArgumentNullException(nameof(scenario));

    try
    {
      _logger.LogDebug($"{nameof(UpdateScenario)} starting. datasource={datasource}, scenarioId={scenario.Id}");

      var fluent = new FluentClient(_httpClient);

      var response = await fluent
        .PutAsync($"scenarios/{datasource}/{scenario.Id}")
        .WithBody(scenario)
        .WithCancellationToken(cancellationToken)
        .WithApiExceptionHandling<ScenarioDTO>(); // ← Použití extension metody

      _logger.LogInformation($"{nameof(UpdateScenario)} completed. datasource={datasource}, scenarioId={scenario.Id}");

      return response;
    }
    catch (ApiValidationException vex)
    {
      _logger.LogWarning($"{nameof(UpdateScenario)} validation error. datasource={datasource}, errors={vex.Errors.Count}");
      throw;
    }
    catch (ApiException aex)
    {
      _logger.LogError(aex, $"{nameof(UpdateScenario)} API error. datasource={datasource}, status={aex.StatusCode}");
      throw;
    }
    catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
    {
      _logger.LogInformation($"{nameof(UpdateScenario)} cancelled by token. datasource={datasource}, scenarioId={scenario.Id}");
      throw;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Unexpected error in {nameof(UpdateScenario)}. datasource={datasource}, scenarioId={scenario.Id}");
      throw;
    }
  }

  /// <inheritdoc/>
  public async Task<Result<ScenarioDTO>> UpdateScenarioResult(Datasource datasource, ScenarioDTO scenario, CancellationToken cancellationToken)
  {
    if (!Enum.IsDefined(typeof(Datasource), datasource))
      return Result<ScenarioDTO>.Invalid(new ValidationError(nameof(datasource), "Neplatný datasource."));
    
    if (scenario is null)
      return Result<ScenarioDTO>.Invalid(new ValidationError(nameof(scenario), "Scenario nesmí být null."));

    _logger.LogDebug($"{nameof(UpdateScenarioResult)} starting. datasource={datasource}, scenarioId={scenario.Id}");

    var fluent = new FluentClient(_httpClient);

    var result = await fluent
      .PutAsync($"scenarios/{datasource}/{scenario.Id}")
      .WithBody(scenario)
      .WithCancellationToken(cancellationToken)
      .AsResult<ScenarioDTO>(); // ← Použití nové extension metody

    if (result.IsSuccess)
      _logger.LogInformation($"{nameof(UpdateScenarioResult)} completed. datasource={datasource}, scenarioId={scenario.Id}");
    else
      _logger.LogWarning($"{nameof(UpdateScenarioResult)} failed. datasource={datasource}, status={result.Status}");

    return result;
  }
}
