using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.Deployments;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Contracts.Scenarios;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pathoschild.Http.Client;

namespace AVAIntegrationModeler.API.Client;

/// <summary>
/// Client Integration Modeler pro volání AVA Integration Modeler API.
/// </summary>
public class AVAIntegrationModelerApiClient : IAVAIntegrationModelerApiClient
{
  /// <summary>
  /// Název pojmenovaného HttpClienta s delším timeoutem, používaného pro hromadný import
  /// datových modelů z AVAPlace (viz <see cref="ImportAllDataModelsFromAvaPlace"/>).
  /// </summary>
  public const string BulkImportHttpClientName = "AVAIntegrationModelerApiClient.BulkImport";

  /// <summary>
  /// Http client
  /// </summary>
  private readonly HttpClient _httpClient;

  /// <summary>
  /// Továrna pro vytváření pojmenovaných HttpClientů (viz <see cref="BulkImportHttpClientName"/>).
  /// </summary>
  private readonly IHttpClientFactory _httpClientFactory;

  /// <summary>
  /// Logger
  /// </summary>
  private readonly ILogger<AVAIntegrationModelerApiClient> _logger;

  public AVAIntegrationModelerApiClient(
    HttpClient httpClient,
    IHttpClientFactory httpClientFactory,
    ILogger<AVAIntegrationModelerApiClient> logger)
  {
    _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
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
    var requestbody = new
    {
      Datasource = datasource,
      Scenario = scenario
    };
    if (!Enum.IsDefined(typeof(Datasource), datasource))
      return Result<Guid>.Invalid(new ValidationError(nameof(datasource), "Neplatný datasource."));
    
    if (scenario is null)
      return Result<Guid>.Invalid(new ValidationError(nameof(scenario), "Scenario nesmí být null."));

    _logger.LogDebug($"{nameof(CreateScenario)} starting. datasource={datasource}, scenarioCode={scenario.Code}");

    var fluent = new FluentClient(_httpClient);

    var result = await fluent
      .PostAsync("scenarios")
      .WithBody(requestbody)
      .WithCancellationToken(cancellationToken)
      .AsResult<Guid>(); // ← Použití nové extension metody

    if (result.IsSuccess)
      _logger.LogInformation($"{nameof(CreateScenario)} completed. datasource={datasource}, scenarioId={result.Value}");
    else
      _logger.LogWarning($"{nameof(CreateScenario)} failed. datasource={datasource}, status={result.Status}");

    return result;
  }

  /// <inheritdoc/>
  public async Task<Result<ScenarioDTO>> UpdateScenario(Datasource datasource, ScenarioDTO scenario, CancellationToken cancellationToken)
  {
    if (!Enum.IsDefined(typeof(Datasource), datasource))
      return Result<ScenarioDTO>.Invalid(new ValidationError(nameof(datasource), "Neplatný datasource."));
    
    if (scenario is null)
      return Result<ScenarioDTO>.Invalid(new ValidationError(nameof(scenario), "Scenario nesmí být null."));

    _logger.LogDebug($"{nameof(UpdateScenario)} starting. datasource={datasource}, scenarioId={scenario.Id}");

    var fluent = new FluentClient(_httpClient);
    var requestBody = new
    {
      ScenarioCode = scenario.Code,
      Datasource = datasource,
      Scenario = scenario
    };
    // volám update scénáře podle jeho kódu, protože DataService zatím nemá Identifikátory
    var result = await fluent
      .PutAsync($"scenarios/{datasource}/{scenario.Code}")
      .WithBody(requestBody)
      .WithCancellationToken(cancellationToken)
      .AsResult<ScenarioDTO>(); // ← Použití nové extension metody

    if (result.IsSuccess)
      _logger.LogInformation($"{nameof(UpdateScenario)} completed. datasource={datasource}, scenarioId={scenario.Id}");
    else
      _logger.LogWarning($"{nameof(UpdateScenario)} failed. datasource={datasource}, status={result.Status}");

    return result;
  }

  public async Task<Result> DeleteScenario(Datasource datasource, string scenarioCode, CancellationToken ct)
  {
    _logger.LogDebug($"{nameof(DeleteScenario)} starting. datasource={datasource}, scenarioCode={scenarioCode}");

    var fluent = new FluentClient(_httpClient);

    var result = await fluent
      .DeleteAsync($"scenarios/{datasource}/{Uri.EscapeDataString(scenarioCode)}")
      .WithCancellationToken(ct)
      .AsResult<object>();

    return result.IsSuccess ? Result.NoContent() : Result.Error(string.Join("; ", result.Errors));
  }

  /// <inheritdoc/>
  public async Task<DataModelDTO> GetDataModel(Datasource datasource, Guid dataModelId, CancellationToken cancellationToken)
  {
    _logger.LogDebug($"{nameof(GetDataModel)} starting. datasource={datasource}, dataModelId={dataModelId}");
    var fluent = new FluentClient(_httpClient);
    var response = await fluent
      .GetAsync($"datamodels/{datasource}/{dataModelId}")
      .WithCancellationToken(cancellationToken)
      .As<DataModelDTO>();
    return response ?? new DataModelDTO();
  }

  /// <inheritdoc/>
  public async Task<Result<Guid>> CreateDataModel(Datasource datasource, DataModelDTO dataModel, CancellationToken cancellationToken)
  {
    if (dataModel is null)
      return Result<Guid>.Invalid(new ValidationError(nameof(dataModel), "DataModel nesmí být null."));

    _logger.LogDebug($"{nameof(CreateDataModel)} starting. datasource={datasource}, code={dataModel.Code}");
    var fluent = new FluentClient(_httpClient);
    var result = await fluent
      .PostAsync("datamodels")
      .WithBody(new { Datasource = datasource, DataModel = dataModel })
      .WithCancellationToken(cancellationToken)
      .AsResult<Guid>();

    if (result.IsSuccess)
      _logger.LogInformation($"{nameof(CreateDataModel)} completed. dataModelId={result.Value}");
    else
      _logger.LogWarning($"{nameof(CreateDataModel)} failed. status={result.Status}");

    return result;
  }

  /// <inheritdoc/>
  public async Task<Result<DataModelDTO>> UpdateDataModel(Datasource datasource, DataModelDTO dataModel, CancellationToken cancellationToken)
  {
    if (dataModel is null)
      return Result<DataModelDTO>.Invalid(new ValidationError(nameof(dataModel), "DataModel nesmí být null."));

    _logger.LogDebug($"{nameof(UpdateDataModel)} starting. datasource={datasource}, dataModelId={dataModel.Id}");
    var fluent = new FluentClient(_httpClient);
    // Endpoint vrací UpdateDataModelResponse { DataModel = dto } — musíme přečíst wrapper a rozbalit.
    var rawResult = await fluent
      .PutAsync($"datamodels/{datasource}/{dataModel.Id}")
      .WithBody(new { Datasource = datasource, DataModelId = dataModel.Id, DataModel = dataModel })
      .WithCancellationToken(cancellationToken)
      .AsResult<UpdateDataModelResponse>();

    if (rawResult.IsSuccess)
    {
      _logger.LogInformation($"{nameof(UpdateDataModel)} completed. dataModelId={dataModel.Id}");
      return Result<DataModelDTO>.Success(rawResult.Value.DataModel);
    }

    _logger.LogWarning($"{nameof(UpdateDataModel)} failed. status={rawResult.Status}");
    return rawResult.Status switch
    {
      ResultStatus.NotFound => Result<DataModelDTO>.NotFound(),
      ResultStatus.Invalid => Result<DataModelDTO>.Invalid(rawResult.ValidationErrors),
      ResultStatus.Conflict => Result<DataModelDTO>.Conflict(string.Join("; ", rawResult.Errors)),
      _ => Result<DataModelDTO>.Error(string.Join("; ", rawResult.Errors))
    };
  }

  /// <inheritdoc/>
  public async Task<byte[]> ExportDataModels(Datasource datasource, List<Guid> modelIds, CancellationToken cancellationToken)
  {
    _logger.LogDebug($"{nameof(ExportDataModels)} starting. datasource={datasource}, count={modelIds?.Count}");
    var response = await _httpClient.PostAsJsonAsync(
      "datamodels/export",
      new { Datasource = datasource, ModelIds = modelIds },
      cancellationToken);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsByteArrayAsync(cancellationToken);
  }

  /// <inheritdoc/>
  public async Task<Result> DeleteDataModel(Datasource datasource, Guid dataModelId, CancellationToken cancellationToken)
  {
    _logger.LogDebug($"{nameof(DeleteDataModel)} starting. datasource={datasource}, dataModelId={dataModelId}");
    var fluent = new FluentClient(_httpClient);
    var result = await fluent
      .DeleteAsync($"datamodels/{datasource}/{dataModelId}")
      .WithCancellationToken(cancellationToken)
      .AsResult<object>();

    return result.IsSuccess ? Result.NoContent() : Result.Error(string.Join("; ", result.Errors));
  }

  /// <inheritdoc/>
  public async Task<Result<DeleteAllDataModelsResponse>> DeleteAllDataModels(CancellationToken cancellationToken)
  {
    _logger.LogDebug($"{nameof(DeleteAllDataModels)} starting.");
    var fluent = new FluentClient(_httpClient);
    var result = await fluent
      .DeleteAsync("datamodels/all")
      .WithCancellationToken(cancellationToken)
      .AsResult<DeleteAllDataModelsResponse>();

    if (result.IsSuccess)
      _logger.LogInformation($"{nameof(DeleteAllDataModels)} completed. deletedModels={result.Value.DeletedModelsCount}, deletedRecords={result.Value.DeletedRecordsCount}");
    else
      _logger.LogWarning($"{nameof(DeleteAllDataModels)} failed. status={result.Status}");

    return result;
  }

  /// <inheritdoc/>
  public async Task<DataModelRecordListResponse> GetDataModelRecords(Datasource datasource, Guid? modelId, CancellationToken cancellationToken)
  {
    _logger.LogDebug($"{nameof(GetDataModelRecords)} starting. datasource={datasource}, modelId={modelId}");
    var fluent = new FluentClient(_httpClient);
    var request = fluent.GetAsync("datamodelrecords").WithArgument("datasource", datasource);
    if (modelId.HasValue) request = request.WithArgument("modelId", modelId.Value);
    var response = await request.WithCancellationToken(cancellationToken).As<DataModelRecordListResponse>();
    return response ?? new DataModelRecordListResponse();
  }

  /// <inheritdoc/>
  public async Task<DataModelRecordDTO> GetDataModelRecord(Datasource datasource, Guid recordId, CancellationToken cancellationToken)
  {
    _logger.LogDebug($"{nameof(GetDataModelRecord)} starting. datasource={datasource}, recordId={recordId}");
    var fluent = new FluentClient(_httpClient);
    var response = await fluent
      .GetAsync($"datamodelrecords/{datasource}/{recordId}/detail")
      .WithCancellationToken(cancellationToken)
      .As<DataModelRecordDTO>();
    return response ?? new DataModelRecordDTO();
  }

  /// <inheritdoc/>
  public async Task<Result<Guid>> CreateDataModelRecord(Datasource datasource, DataModelRecordDTO record, CancellationToken cancellationToken)
  {
    if (record is null)
      return Result<Guid>.Invalid(new ValidationError(nameof(record), "Record nesmí být null."));
    _logger.LogDebug($"{nameof(CreateDataModelRecord)} starting. datasource={datasource}");
    var fluent = new FluentClient(_httpClient);
    var result = await fluent
      .PostAsync("datamodelrecords")
      .WithBody(new { Datasource = datasource, Record = record })
      .WithCancellationToken(cancellationToken)
      .AsResult<Guid>();
    return result;
  }

  /// <inheritdoc/>
  public async Task<Result<Guid>> UpdateDataModelRecord(Datasource datasource, DataModelRecordDTO record, CancellationToken cancellationToken)
  {
    if (record is null)
      return Result<Guid>.Invalid(new ValidationError(nameof(record), "Record nesmí být null."));
    _logger.LogDebug($"{nameof(UpdateDataModelRecord)} starting. datasource={datasource}, recordId={record.Id}");
    var fluent = new FluentClient(_httpClient);
    var result = await fluent
      .PutAsync($"datamodelrecords/{datasource}/{record.Id}")
      .WithBody(new { Datasource = datasource, RecordId = record.Id, Record = record })
      .WithCancellationToken(cancellationToken)
      .AsResult<Guid>();
    return result;
  }

  /// <inheritdoc/>
  public async Task<byte[]> ExportDataModelRecords(Datasource datasource, List<Guid> recordIds, CancellationToken cancellationToken)
  {
    _logger.LogDebug($"{nameof(ExportDataModelRecords)} starting. datasource={datasource}, count={recordIds?.Count}");
    var response = await _httpClient.PostAsJsonAsync(
      "datamodelrecords/export",
      new { Datasource = datasource, RecordIds = recordIds },
      cancellationToken);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsByteArrayAsync(cancellationToken);
  }

  /// <inheritdoc/>
  public async Task<Result> DeleteDataModelRecord(Datasource datasource, Guid recordId, CancellationToken cancellationToken)
  {
    _logger.LogDebug($"{nameof(DeleteDataModelRecord)} starting. datasource={datasource}, recordId={recordId}");
    var fluent = new FluentClient(_httpClient);
    var result = await fluent
      .DeleteAsync($"datamodelrecords/{datasource}/{recordId}")
      .WithCancellationToken(cancellationToken)
      .AsResult<object>();
    return result.IsSuccess ? Result.NoContent() : Result.Error(string.Join("; ", result.Errors));
  }

  /// <inheritdoc/>
  public async Task<Result<Guid>> ImportDataModelFromAvaPlace(Guid avaPlaceModelId, CancellationToken cancellationToken)
  {
    if (avaPlaceModelId == Guid.Empty)
      return Result<Guid>.Invalid(new ValidationError(nameof(avaPlaceModelId), "AvaPlaceModelId nesmí být prázdný."));

    _logger.LogDebug($"{nameof(ImportDataModelFromAvaPlace)} starting. avaPlaceModelId={avaPlaceModelId}");
    var fluent = new FluentClient(_httpClient);
    var result = await fluent
      .PostAsync(ImportDataModelFromAvaPlaceRequest.Route)
      .WithBody(new { AvaPlaceModelId = avaPlaceModelId })
      .WithCancellationToken(cancellationToken)
      .AsResult<Guid>();

    if (result.IsSuccess)
      _logger.LogInformation($"{nameof(ImportDataModelFromAvaPlace)} completed. avaPlaceModelId={avaPlaceModelId}, localModelId={result.Value}");
    else
      _logger.LogWarning($"{nameof(ImportDataModelFromAvaPlace)} failed. avaPlaceModelId={avaPlaceModelId}, status={result.Status}");

    return result;
  }

  /// <inheritdoc/>
  public async Task<Result<ImportAllDataModelsFromAvaPlaceResponse>> ImportAllDataModelsFromAvaPlace(CancellationToken cancellationToken)
  {
    _logger.LogDebug($"{nameof(ImportAllDataModelsFromAvaPlace)} starting.");
    // Vlastní HttpClient s delším timeoutem — import velkého počtu modelů z AVAPlace
    // může trvat výrazně déle než běžný timeout ostatních volání API.
    var bulkImportHttpClient = _httpClientFactory.CreateClient(BulkImportHttpClientName);
    var fluent = new FluentClient(bulkImportHttpClient);
    var result = await fluent
      .PostAsync(ImportAllDataModelsFromAvaPlaceRequest.Route)
      .WithBody(new { })
      .WithCancellationToken(cancellationToken)
      .AsResult<ImportAllDataModelsFromAvaPlaceResponse>();

    if (result.IsSuccess)
      _logger.LogInformation($"{nameof(ImportAllDataModelsFromAvaPlace)} completed. success={result.Value.SuccessCount}, failed={result.Value.FailedCount}");
    else
      _logger.LogWarning($"{nameof(ImportAllDataModelsFromAvaPlace)} failed. status={result.Status}");

    return result;
  }

  /// <inheritdoc/>
  public async Task<Contracts.Areas.AreaListResponse> GetAreas(Datasource datasource, CancellationToken cancellationToken)
  {
    _logger.LogDebug($"{nameof(GetAreas)} starting. datasource={datasource}");
    var fluent = new FluentClient(_httpClient);
    var response = await fluent
      .GetAsync("areas")
      .WithArgument("datasource", datasource)
      .WithCancellationToken(cancellationToken)
      .As<Contracts.Areas.AreaListResponse>();
    return response ?? new Contracts.Areas.AreaListResponse();
  }

  /// <inheritdoc/>
  public async Task<AreaDTO> GetArea(Datasource datasource, Guid areaId, CancellationToken cancellationToken)
  {
    _logger.LogDebug($"{nameof(GetArea)} starting. datasource={datasource}, areaId={areaId}");
    var fluent = new FluentClient(_httpClient);
    var response = await fluent
      .GetAsync($"areas/by-id/{datasource}/{areaId}")
      .WithCancellationToken(cancellationToken)
      .As<AreaDTO>();
    return response ?? new AreaDTO();
  }

  /// <inheritdoc/>
  public async Task<AreaDTO> GetArea(Datasource datasource, string areaCode, CancellationToken cancellationToken)
  {
    _logger.LogDebug($"{nameof(GetArea)} starting. datasource={datasource}, areaCode={areaCode}");
    var fluent = new FluentClient(_httpClient);
    var response = await fluent
      .GetAsync($"areas/{datasource}/{Uri.EscapeDataString(areaCode)}")
      .WithCancellationToken(cancellationToken)
      .As<AreaDTO>();
    return response ?? new AreaDTO();
  }

  /// <inheritdoc/>
  public async Task<Result<Guid>> CreateArea(Datasource datasource, AreaDTO area, CancellationToken cancellationToken)
  {
    if (area is null)
      return Result<Guid>.Invalid(new ValidationError(nameof(area), "Oblast nesmí být null."));
    _logger.LogDebug($"{nameof(CreateArea)} starting. datasource={datasource}");
    var fluent = new FluentClient(_httpClient);
    var result = await fluent
      .PostAsync("areas")
      .WithBody(new { Datasource = datasource, Area = area })
      .WithCancellationToken(cancellationToken)
      .AsResult<Guid>();
    return result;
  }

  /// <inheritdoc/>
  public async Task<Result<AreaDTO>> UpdateArea(Datasource datasource, AreaDTO area, CancellationToken cancellationToken)
  {
    if (area is null)
      return Result<AreaDTO>.Invalid(new ValidationError(nameof(area), "Oblast nesmí být null."));
    _logger.LogDebug($"{nameof(UpdateArea)} starting. datasource={datasource}, areaCode={area.Code}");
    var fluent = new FluentClient(_httpClient);
    var result = await fluent
      .PutAsync($"areas/{datasource}/{Uri.EscapeDataString(area.Code)}")
      .WithBody(new { Datasource = datasource, AreaCode = area.Code, Area = area })
      .WithCancellationToken(cancellationToken)
      .AsResult<AreaDTO>();
    return result;
  }

  /// <inheritdoc/>
  public async Task<Result> DeleteArea(Datasource datasource, string areaCode, CancellationToken cancellationToken)
  {
    _logger.LogDebug($"{nameof(DeleteArea)} starting. datasource={datasource}, areaCode={areaCode}");
    var fluent = new FluentClient(_httpClient);
    var result = await fluent
      .DeleteAsync($"areas/{datasource}/{Uri.EscapeDataString(areaCode)}")
      .WithCancellationToken(cancellationToken)
      .AsResult<object>();
    return result.IsSuccess ? Result.NoContent() : Result.Error(string.Join("; ", result.Errors));
  }

  /// <inheritdoc/>
  public async Task<DeploymentListResponse> GetDeployments(CancellationToken cancellationToken)
  {
    _logger.LogDebug($"{nameof(GetDeployments)} starting.");
    var fluent = new FluentClient(_httpClient);
    var response = await fluent
      .GetAsync("deployments")
      .WithCancellationToken(cancellationToken)
      .As<DeploymentListResponse>();
    return response ?? new DeploymentListResponse();
  }

  /// <inheritdoc/>
  public async Task<DeploymentDTO> GetDeployment(Guid id, CancellationToken cancellationToken)
  {
    _logger.LogDebug($"{nameof(GetDeployment)} starting. id={id}");
    var fluent = new FluentClient(_httpClient);
    var response = await fluent
      .GetAsync($"deployments/by-id/{id}")
      .WithCancellationToken(cancellationToken)
      .As<DeploymentDTO>();
    return response ?? new DeploymentDTO();
  }

  /// <inheritdoc/>
  public async Task<DeploymentDTO> GetDeployment(string code, CancellationToken cancellationToken)
  {
    _logger.LogDebug($"{nameof(GetDeployment)} starting. code={code}");
    var fluent = new FluentClient(_httpClient);
    var response = await fluent
      .GetAsync($"deployments/{Uri.EscapeDataString(code)}")
      .WithCancellationToken(cancellationToken)
      .As<DeploymentDTO>();
    return response ?? new DeploymentDTO();
  }

  /// <inheritdoc/>
  public async Task<Result<Guid>> CreateDeployment(DeploymentDTO deployment, CancellationToken cancellationToken)
  {
    if (deployment is null)
      return Result<Guid>.Invalid(new ValidationError(nameof(deployment), "Nasazení nesmí být null."));
    _logger.LogDebug($"{nameof(CreateDeployment)} starting. code={deployment.Code}");
    var fluent = new FluentClient(_httpClient);
    var result = await fluent
      .PostAsync("deployments")
      .WithBody(new { Deployment = deployment })
      .WithCancellationToken(cancellationToken)
      .AsResult<Guid>();
    return result;
  }

  /// <inheritdoc/>
  public async Task<Result<DeploymentDTO>> UpdateDeployment(DeploymentDTO deployment, CancellationToken cancellationToken)
  {
    if (deployment is null)
      return Result<DeploymentDTO>.Invalid(new ValidationError(nameof(deployment), "Nasazení nesmí být null."));
    _logger.LogDebug($"{nameof(UpdateDeployment)} starting. id={deployment.Id}");
    var fluent = new FluentClient(_httpClient);
    var result = await fluent
      .PutAsync($"deployments/{Uri.EscapeDataString(deployment.Code)}")
      .WithBody(new { DeploymentCode = deployment.Code, Deployment = deployment })
      .WithCancellationToken(cancellationToken)
      .AsResult<DeploymentDTO>();
    return result;
  }

  /// <inheritdoc/>
  public async Task<Result> DeleteDeployment(string code, CancellationToken cancellationToken)
  {
    _logger.LogDebug($"{nameof(DeleteDeployment)} starting. code={code}");
    var fluent = new FluentClient(_httpClient);
    var result = await fluent
      .DeleteAsync($"deployments/{Uri.EscapeDataString(code)}")
      .WithCancellationToken(cancellationToken)
      .AsResult<object>();
    return result.IsSuccess ? Result.NoContent() : Result.Error(string.Join("; ", result.Errors));
  }

  /// <inheritdoc/>
  public async Task<byte[]> ExportDeployment(string deploymentCode, CancellationToken cancellationToken)
  {
    _logger.LogDebug($"{nameof(ExportDeployment)} starting. deploymentCode={deploymentCode}");
    var response = await _httpClient.PostAsJsonAsync(
      $"deployments/{Uri.EscapeDataString(deploymentCode)}/export",
      new { },
      cancellationToken);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsByteArrayAsync(cancellationToken);
  }
}
