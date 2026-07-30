using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASOL.Core.Identity;
using ASOL.Core.Identity.Options;
using ASOL.DataService.Contracts;
using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.AVAPlace.Options;
using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RTX = AVAIntegrationModeler.AVAPlace.ServiceRuntimeTenantContext;

namespace AVAIntegrationModeler.AVAPlace.Providers;

/// <summary>
/// Implementace poskytovatele dat pro integrace. <see cref="IIntegrationDataProvider"/>
/// </summary>
public class IntegrationDataProvider : IIntegrationDataProvider
{
  private readonly IServiceProvider _serviceProvider;
  private readonly IOptions<AVAPlaceOptions> _avaPlaceOptions;
  private readonly IRuntimeContext _runtimeContext;


  /// <summary>
  /// Základní konstruktor.
  /// </summary>
  /// <param name="serviceProvider"><see cref="IServiceProvider"/></param>
  /// <exception cref="ArgumentNullException"></exception>
  public IntegrationDataProvider(
    IServiceProvider serviceProvider,
    IOptions<Options.AVAPlaceOptions> avaPlaceOptions
    )
  {
    _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    _avaPlaceOptions = avaPlaceOptions;
    _runtimeContext = _serviceProvider.GetRequiredService<IRuntimeContext>() ?? throw new ArgumentNullException(nameof(IRuntimeContext));
  }

  /// <summary>
  /// Identifikátor tenanta.
  /// </summary>
  private string tenantId
  {
    get
    {
      var result = _runtimeContext?.Security?.TenantId!;
      if (string.IsNullOrEmpty(result))
      {
        result = _avaPlaceOptions.Value.TenantId;
      }
      return result;
    }
  }

  /// <inheritdoc/>
  public async Task<IEnumerable<FeatureSummaryDTO>> GetFeaturesSummaryAsync(CancellationToken ct = default)
  {
    List<FeatureSummaryDTO> features = new List<FeatureSummaryDTO>();

    string tenantId = _runtimeContext?.Security?.TenantId!;
    if (string.IsNullOrEmpty(tenantId))
    {
      tenantId = _avaPlaceOptions.Value.TenantId;
    }
    return await RTX.ExecuteInContextAsync<ICustomDataServiceClient, IEnumerable<FeatureSummaryDTO>>(
      _serviceProvider,
      tenantId,
      async client =>
      {
        List<FeatureSummaryDTO> featureSummaries = new List<FeatureSummaryDTO>();
        // Replace with actual logic to get features, e.g.:
        var dataServiceResult = await client.IntegrationFeatures.GetFeaturesAsync(
         new ASOL.DataService.Contracts.Filters.IntegrationFeatureFilter()
         {
         },
         new ASOL.Core.Paging.Contracts.Filters.PagingFilter()
         {
           Offset = 0,
           Limit = int.MaxValue
         }, new ASOL.Core.Domain.Contracts.BaseEntityFilter()
         {
           Released = true,
           Deleted = false,
         },
         ct);
        if (dataServiceResult != null && dataServiceResult.Any())
        {
          foreach (var scenario in dataServiceResult)
          {
            featureSummaries.Add(Mapping.FeatureMapper.FeatureSummaryDTO(scenario));
          }
        }
        return featureSummaries;
      }
    );
  }

  /// <inheritdoc/>
  public async Task<IEnumerable<FeatureDTO>> GetFeaturesAsync(CancellationToken ct = default)
  {
    List<FeatureDTO> features = new List<FeatureDTO>();

    return await RTX.ExecuteInContextAsync<ICustomDataServiceClient, IEnumerable<FeatureDTO>>(
      _serviceProvider,
      tenantId,
      async client =>
      {
        List<DataModelSummaryDTO> modelSummaryDTOs = new List<DataModelSummaryDTO>();
        var modelResult = await client.GetDataModelsAsync(
         new ASOL.Core.Paging.Contracts.Filters.PagingFilter()
         {
           Offset = 0,
           Limit = int.MaxValue
         },
         ct);

        modelResult.ToList().ForEach(model =>
        {
          modelSummaryDTOs.Add(Mapping.DataModelMapper.MapToSummaryDTO(model));
        });

        List<FeatureDTO> features = new List<FeatureDTO>();

        // Replace with actual logic to get features, e.g.:
        var dataServiceResult = await client.IntegrationFeatures.GetFeaturesAsync(
         new ASOL.DataService.Contracts.Filters.IntegrationFeatureFilter()
         {
         },
         new ASOL.Core.Paging.Contracts.Filters.PagingFilter()
         {
           Offset = 0,
           Limit = int.MaxValue
         }, new ASOL.Core.Domain.Contracts.BaseEntityFilter()
         {
           Released = true,
           Deleted = false,
         },
         ct);

        List<FeatureSummaryDTO> featureSummaryDTOs = new List<FeatureSummaryDTO>();
        dataServiceResult.ToList().ForEach(feature =>
        {
          featureSummaryDTOs.Add(Mapping.FeatureMapper.FeatureSummaryDTO(feature));
        });

        if (dataServiceResult != null && dataServiceResult.Any())
        {

          List<IntegrationFeatureModel> integrationFeatures = new List<IntegrationFeatureModel>();
          foreach (var featureSummary in dataServiceResult)
          {
            integrationFeatures.Add(await client.IntegrationFeatures.GetFeatureAsync(featureSummary.Id, true, ct));
          }

          foreach (var integrationFeatureModel in integrationFeatures)
          {
            features.Add(Mapping.FeatureMapper.FeatureDTO(integrationFeatureModel, featureSummaryDTOs, modelSummaryDTOs));
          }
        }
        return features;
      }
    );
  }

  /// <inheritdoc/>
  public async Task<IEnumerable<ScenarioDTO>> GetScenarios(CancellationToken ct = default)
  {

    return await RTX.ExecuteInContextAsync<ICustomDataServiceClient, IEnumerable<ScenarioDTO>>(
      _serviceProvider,
      tenantId,
      async client =>
      {
        List<ScenarioDTO> scenarios = new List<ScenarioDTO>();
        // Replace with actual logic to get features, e.g.:
        var dataServiceResult = await client.IntegrationScenarios.GetScenariosAsync(
         new ASOL.DataService.Contracts.Filters.IntegrationScenarioFilter()
         {
         },
         new ASOL.Core.Paging.Contracts.Filters.PagingFilter()
         {
           Offset = 0,
           Limit = int.MaxValue
         }, new ASOL.Core.Domain.Contracts.BaseEntityFilter()
         {
           Released = true,
           Deleted = false,
         },
         ct);
        if (dataServiceResult != null && dataServiceResult.Any())
        {
          foreach (var scenario in dataServiceResult)
          {
            scenarios.Add(Mapping.ScenarioMapper.MapToDTO(scenario));
          }
        }
        return scenarios;
      }
    );
  }


  public async Task<IEnumerable<ScenarioDTO>> GetAreasAsync(CancellationToken ct = default)
  {

    return await RTX.ExecuteInContextAsync<ICustomDataServiceClient, IEnumerable<ScenarioDTO>>(
      _serviceProvider,
      tenantId,
      async client =>
      {
        //ASOL.DataService.Contracts.IntegrationMapByAreaDefinition
        //client.Are
        List<ScenarioDTO> scenarios = new List<ScenarioDTO>();
        // Replace with actual logic to get features, e.g.:
        var dataServiceResult = await client.IntegrationScenarios.GetScenariosAsync(
         new ASOL.DataService.Contracts.Filters.IntegrationScenarioFilter()
         {
         },
         new ASOL.Core.Paging.Contracts.Filters.PagingFilter()
         {
           Offset = 0,
           Limit = int.MaxValue
         }, new ASOL.Core.Domain.Contracts.BaseEntityFilter()
         {
           Released = true,
           Deleted = false,
         },
         ct);
        if (dataServiceResult != null && dataServiceResult.Any())
        {
          foreach (var scenario in dataServiceResult)
          {
            scenarios.Add(Mapping.ScenarioMapper.MapToDTO(scenario));
          }
        }
        return scenarios;
      }
    );
  }

  /// <inheritdoc/>
  public async Task<bool> CreateScenario(ScenarioDTO scenario, CancellationToken cancelationToken = default)
  {
    return await importScenario(scenario, false, cancelationToken);
  }

  /// <inheritdoc/>
  public async Task<bool> UpdateScenario(ScenarioDTO scenario, CancellationToken cancelationToken = default)
  {
    return await importScenario(scenario, true, cancelationToken);
  }

  /// <inheritdoc/>
  public async Task<bool> DeleteScenario(string scenarioCode, CancellationToken cancelationToken = default)
  {
    return await RTX.ExecuteInContextAsync<ICustomDataServiceClient, bool>(
      _serviceProvider,
      tenantId,
      async client =>
      {
        List<ScenarioDTO> scenarios = new List<ScenarioDTO>();

        var imported = await client.IntegrationScenarios.DeleteScenarioAsync(scenarioCode, cancelationToken);

        return imported;
      }
    );
  }

  /// <summary>
  /// importuje scénář do DataService
  /// </summary>
  /// <param name="scenario">Integrační scénář</param>
  /// <param name="allowUdate">Příznak, že se má povolit aktualizace existujícího scénáře</param>
  /// <param name="cancelationToken"><see cref="CancellationToken"/></param>
  /// <returns>Příznak, že import byl úspěšný</returns>
  private async Task<bool> importScenario(ScenarioDTO scenario, bool allowUdate, CancellationToken cancelationToken = default)
  {
    IntegrationScenarioDefinition integrationScenarioDefinition = new IntegrationScenarioDefinition();
    integrationScenarioDefinition = Mapping.ScenarioMapper.MapToIntegrationScenarioDefinition(scenario);

    return await RTX.ExecuteInContextAsync<ICustomDataServiceClient, bool>(
      _serviceProvider,
      tenantId,
      async client =>
      {
        List<ScenarioDTO> scenarios = new List<ScenarioDTO>();

        var imported = await client.IntegrationScenarios.ImportScenarioAsync(integrationScenarioDefinition, allowUdate, false, cancelationToken);

        return imported;
      }
    );
  }

  /// <inheritdoc/>
  public async Task<IEnumerable<DataModelDTO>> GetDataModelsAsync(CancellationToken ct = default)
  {
    return await RTX.ExecuteInContextAsync<ICustomDataServiceClient, IEnumerable<DataModelDTO>>(
      _serviceProvider,
      tenantId,
      async client =>
      {
        List<DataModelDTO> dataModels = new List<DataModelDTO>();
        // Replace with actual logic to get features, e.g.:
        var dataServiceResult = await client.GetDataModelsAsync(

         new ASOL.Core.Paging.Contracts.Filters.PagingFilter()
         {
           Offset = 0,
           Limit = int.MaxValue
         },
         ct);

        if (dataServiceResult != null && dataServiceResult.Any())
        {
          foreach (var dataModel in dataServiceResult)
          {
            dataModels.Add(Mapping.DataModelMapper.MapToDTO(dataModel));
          }
        }
        return dataModels;
      }
    );
  }

  /// <inheritdoc/>
  public async Task<IEnumerable<DataModelSummaryDTO>> GetDataModelsSummaryAsync(CancellationToken ct = default)
  {
    return await RTX.ExecuteInContextAsync<ICustomDataServiceClient, IEnumerable<DataModelSummaryDTO>>(
      _serviceProvider,
      tenantId,
      async client =>
      {
        List<DataModelSummaryDTO> scenarios = new List<DataModelSummaryDTO>();
        // Replace with actual logic to get features, e.g.:
        var dataServiceResult = await client.GetDataModelsAsync(

         new ASOL.Core.Paging.Contracts.Filters.PagingFilter()
         {
           Offset = 0,
           Limit = int.MaxValue
         },
         ct);

        if (dataServiceResult != null && dataServiceResult.Any())
        {
          foreach (var dataModel in dataServiceResult)
          {
            scenarios.Add(Mapping.DataModelMapper.MapToSummaryDTO(dataModel));
          }
        }
        return scenarios;
      }
    );
  }

  /// <inheritdoc/>
  public async Task<IEnumerable<IntegrationMapSummaryDTO>> GetIntegrationMapSummaryAsync(CancellationToken ct = default)
  {
    string tenantId = _runtimeContext?.Security?.TenantId!;
    if (string.IsNullOrEmpty(tenantId))
    {
      tenantId = _avaPlaceOptions.Value.TenantId;
    }

    return await RTX.ExecuteInContextAsync<ICustomDataServiceClient, IEnumerable<IntegrationMapSummaryDTO>>(
      _serviceProvider,
      tenantId,
      async client =>
      {
        List<IntegrationMapSummaryDTO> integrationMap = new List<IntegrationMapSummaryDTO>();
        // Replace with actual logic to get features, e.g.:
        var dataServiceResult = await client.IntegrationMaps.GetMapsAsync(

         new ASOL.DataService.Contracts.Filters.IntegrationMapFilter()
         {
           // Add any necessary filter criteria here
         },
          new ASOL.Core.Paging.Contracts.Filters.PagingFilter()
          {
            Offset = 0,
            Limit = int.MaxValue
          },
          new ASOL.Core.Domain.Contracts.BaseEntityFilter()
          {
            Released = true,
            Deleted = false,
          },
         ct);

        if (dataServiceResult != null && dataServiceResult.Any())
        {
          foreach (var dataModel in dataServiceResult)
          {
            integrationMap.Add(Mapping.IntegrationMapMapper.MapToSummaryDTO(dataModel));
          }
        }
        return integrationMap;
      }
    );
  }

  /// <inheritdoc/>

  public async Task<ScenarioDTO> GetScenario(Guid scenarioId, CancellationToken ct = default)
  {
    var avaResult = await RTX.ExecuteInContextAsync<ICustomDataServiceClient, IntegrationScenarioModel>(
      _serviceProvider,
      tenantId,
      async client =>
      {
        var dataServiceResult = await client.IntegrationScenarios.GetScenarioAsync(
          scenarioId.ToString(), false, ct);
        return dataServiceResult;
      }
    );

    var result = Mapping.ScenarioMapper.MapToDTO(avaResult);
    // totáhnout feature summary atd.
    if (result.InputFeatureId.HasValue)
    {
      var inputFeature = await GetFeaturesSummaryAsync(ct);
      result = result with
      {
        InputFeatureSummary = inputFeature.FirstOrDefault(f => f.Id == result.InputFeatureId.Value)
      };
    }

    if (result.OutputFeatureId.HasValue)
    {
      var outputFeature = await GetFeaturesSummaryAsync(ct);
      result = result with
      {
        OutputFeatureSummary = outputFeature.FirstOrDefault(f => f.Id == result.OutputFeatureId.Value)
      };
    }
    return result;
  }

  /// <inheritdoc/>
  public async Task<FeatureSummaryDTO> GetFeatureSummary(Guid featureId, CancellationToken ct = default)
  {
    FeatureSummaryDTO result = new FeatureSummaryDTO();
    var avaResult = await RTX.ExecuteInContextAsync<ICustomDataServiceClient, IntegrationFeatureModel>(
     _serviceProvider,
     tenantId,
     async client =>
     {
       var dataServiceResult = await client.IntegrationFeatures.GetFeatureAsync(
         featureId.ToString(), true, ct);
       return dataServiceResult;
     });
    if (avaResult == default)
    {
      throw new Ardalis.GuardClauses.NotFoundException(featureId.ToString(), "Feature");
    }

    result = Mapping.FeatureMapper.MapFeatureSummaryDTO(avaResult!);

    return result;
  }

  /// <inheritdoc/>
  public async Task<FeatureSummaryDTO> GetFeatureSummary(string featureCode, CancellationToken ct = default)
  {
    FeatureSummaryDTO result = new FeatureSummaryDTO();
    var avaResult = await RTX.ExecuteInContextAsync<ICustomDataServiceClient, IntegrationFeatureModel>(
     _serviceProvider,
     tenantId,
     async client =>
     {
       var dataServiceResult = await client.IntegrationFeatures.GetFeatureAsync(
         featureCode, true, ct);
       return dataServiceResult;
     });
    if (avaResult == default)
    {
      throw new Ardalis.GuardClauses.NotFoundException(featureCode, "Feature");
    }

    result = Mapping.FeatureMapper.MapFeatureSummaryDTO(avaResult!);

    return result;
  }

  /// <inheritdoc/>
  public async Task<FeatureDTO> GetFeature(string featureCode, CancellationToken ct = default)
  {
    FeatureDTO result = new FeatureDTO();
    var avaResult = await RTX.ExecuteInContextAsync<ICustomDataServiceClient, IntegrationFeatureModel>(
     _serviceProvider,
     tenantId,
     async client =>
     {
       var dataServiceResult = await client.IntegrationFeatures.GetFeatureAsync(
         featureCode, true, ct);
       return dataServiceResult;
     });
    if (avaResult == default)
    {
      throw new Ardalis.GuardClauses.NotFoundException(featureCode, "Feature");
    }

    result = Mapping.FeatureMapper.FeatureDTO(avaResult!);

    return result;
  }
  /// <inheritdoc/>
  public async Task<FeatureDTO> GetFeature(Guid featureId, CancellationToken ct = default)
  {
    FeatureDTO result = new FeatureDTO();
    var avaResult = await RTX.ExecuteInContextAsync<ICustomDataServiceClient, IntegrationFeatureModel>(
     _serviceProvider,
     tenantId,
     async client =>
     {
       var dataServiceResult = await client.IntegrationFeatures.GetFeatureAsync(
         featureId.ToString(), true, ct);
       return dataServiceResult;
     });
    if (avaResult == default)
    {
      throw new Ardalis.GuardClauses.NotFoundException(featureId.ToString(), "Feature");
    }

    result = Mapping.FeatureMapper.FeatureDTO(avaResult!);

    return result;
  }

  /// <inheritdoc/>
  public async Task<ScenarioDTO> GetScenario(string scenarioCode, CancellationToken ct = default)
  {
    var avaResult = await RTX.ExecuteInContextAsync<ICustomDataServiceClient, IntegrationScenarioModel>(
     _serviceProvider,
     tenantId,
     async client =>
     {
       var dataServiceResult = await client.IntegrationScenarios.GetScenarioAsync(
         scenarioCode, false, ct);
       return dataServiceResult;
     }
   );

    var result = Mapping.ScenarioMapper.MapToDTO(avaResult);
    // totáhnout feature summary atd.
    if (result.InputFeatureId.HasValue)
    {
      var inputFeature = await GetFeaturesSummaryAsync(ct);
      result = result with
      {
        InputFeatureSummary = inputFeature.FirstOrDefault(f => f.Id == result.InputFeatureId.Value)
      };
    }

    if (result.OutputFeatureId.HasValue)
    {
      var outputFeature = await GetFeaturesSummaryAsync(ct);
      result = result with
      {
        OutputFeatureSummary = outputFeature.FirstOrDefault(f => f.Id == result.OutputFeatureId.Value)
      };
    }
    return result;
  }

  public Task<IEnumerable<DataModelRecordDTO>> GetDataModelRecordsAsync(Guid modelId, CancellationToken cancelationToken = default)
  {
    // AVAPlace DataService zatím neposkytuje záznamy přes API — vracíme prázdný seznam
    return Task.FromResult(Enumerable.Empty<DataModelRecordDTO>());
  }

  /// <inheritdoc/>
  public async Task<DataModelDTO?> GetDataModelByIdAsync(Guid modelId, CancellationToken ct = default)
  {
    return await RTX.ExecuteInContextAsync<ICustomDataServiceClient, DataModelDTO?>(
      _serviceProvider,
      tenantId,
      async client =>
      {
        var definition = await client.GetDataModelAsync(modelId, true, ct);
        if (definition == null) return null;
        return Mapping.DataModelMapper.MapToDTO(definition);
      }
    );
  }
}
