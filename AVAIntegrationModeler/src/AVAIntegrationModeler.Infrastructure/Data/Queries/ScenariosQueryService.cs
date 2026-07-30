using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Features.Mapping;
using AVAIntegrationModeler.UseCases.Scenarios;
using AVAIntegrationModeler.UseCases.Scenarios.Mapping;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.Extensions.Caching.Memory;

namespace AVAIntegrationModeler.Infrastructure.Data.Queries;

/// <summary>
/// Implementace dotazovací služby pro scénáře.
/// </summary>
/// <param name="databaseContext"><see cref="AppDbContext"/></param>
/// <param name="integrationDataProvider"><see cref="IIntegrationDataProvider"/></param>
/// <param name="memoryCache"></param>
public class ScenariosQueryService(
  AppDbContext databaseContext, 
  IIntegrationDataProvider integrationDataProvider,
  IMemoryCache memoryCache) : IScenariosQueryService, UseCases.ICacheableQueryService
{
  private const string primaryKeyName = "ScenarioListQuery";
  private readonly IMemoryCache _memoryCache = memoryCache;

  private string getChacheKey(Datasource datasource) => $"{primaryKeyName}-{datasource}";

  
  /// <inheritdoc />
  public async Task<IEnumerable<ScenarioDTO>> ListAsync(Datasource datasouce) 
  {
    var methodResult = await _memoryCache.GetOrCreateAsync(
      getChacheKey(datasouce),
      async entry =>
      {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        entry.SlidingExpiration = TimeSpan.FromMinutes(2);
        entry.Priority = CacheItemPriority.Normal;

        List<ScenarioDTO> result;

        if (datasouce == Datasource.AVAPlace)
        {
          // AVAPlace – načtení přes poskytovatele integrací
          var scenariosTask = integrationDataProvider.GetScenarios();
          var featuresTask = integrationDataProvider.GetFeaturesSummaryAsync();
          await Task.WhenAll(scenariosTask, featuresTask);

          var scenarios = scenariosTask.Result.ToList();
          var features = featuresTask.Result.ToDictionary(f => f.Id);

          result = scenarios.Select(s => new ScenarioDTO
          {
            Id = s.Id,
            Code = s.Code,
            Name = s.Name,
            Description = s.Description,
            InputFeatureId = s.InputFeatureId,
            OutputFeatureId = s.OutputFeatureId,
            InputFeatureSummary = s.InputFeatureId.HasValue && features.ContainsKey(s.InputFeatureId.Value)
              ? features[s.InputFeatureId.Value]
              : null,
            OutputFeatureSummary = s.OutputFeatureId.HasValue && features.ContainsKey(s.OutputFeatureId.Value)
              ? features[s.OutputFeatureId.Value]
              : null
          }).ToList();
        }
        else
        {
          // Databáze – explicitní join bez navigačních vlastností
          var scenariosTask = databaseContext.Scenarios.ToListAsync();
          var allFeaturesTask = databaseContext.Features.ToListAsync();
          await Task.WhenAll(scenariosTask, allFeaturesTask);

          var scenarios = scenariosTask.Result;
          var features = allFeaturesTask.Result;
          var featureDict = features.ToDictionary(f => f.Id);

          result = scenarios.Select(s => new ScenarioDTO
          {
            Id = s.Id,
            Code = s.Code,
            Name = s.Name,
            Description = s.Description,
            InputFeatureId = s.InputFeature,
            OutputFeatureId = s.OutputFeature,
            InputFeatureSummary = s.InputFeature.HasValue && featureDict.ContainsKey(s.InputFeature.Value)
              ? new FeatureSummaryDTO { Id = featureDict[s.InputFeature.Value].Id, Code = featureDict[s.InputFeature.Value].Code }
              : null,
            OutputFeatureSummary = s.OutputFeature.HasValue && featureDict.ContainsKey(s.OutputFeature.Value)
              ? new FeatureSummaryDTO { Id = featureDict[s.OutputFeature.Value].Id, Code = featureDict[s.OutputFeature.Value].Code }
              : null
          }).ToList();
        }

        return result;
      });

    return methodResult!;
  }

  /// <summary>
  /// Vrátí scénář podle Id; vyhazuje NotFoundException, pokud neexistuje.
  /// </summary>
  public async Task<ScenarioDTO> GetScenario(Datasource dataSource, Guid scenarioId, CancellationToken ct)
  {
    if (dataSource == Datasource.AVAPlace)
    {
      return await integrationDataProvider.GetScenario(scenarioId);
    }
    else
    {
      var scenario = await databaseContext.Scenarios.FirstOrDefaultAsync(s => s.Id == scenarioId, ct);
      if (scenario == null)
      {
        throw new NotFoundException(scenarioId.ToString(), "Scenario");
      }
      return ScenarioMapper.MapToScenarioDTO(scenario);
    }
  }

  /// <summary>
  /// Vrátí scénář podle kódu; vyhazuje NotFoundException, pokud neexistuje.
  /// </summary>
  public async Task<ScenarioDTO> GetScenario(Datasource dataSource, string scenarioCode, CancellationToken ct)
  {
    if (dataSource == Datasource.AVAPlace)
    {
      return await integrationDataProvider.GetScenario(scenarioCode);
    }
    else
    {
      var scenario = await databaseContext.Scenarios.FirstOrDefaultAsync(s => s.Code == scenarioCode, ct);

      var inputFeature = scenario?.InputFeature != null
        ? await databaseContext.Features.FirstOrDefaultAsync(f => f.Id == scenario.InputFeature, ct)
        : null;

      var outputFeature = scenario?.OutputFeature != null
        ? await databaseContext.Features.FirstOrDefaultAsync(f => f.Id == scenario.OutputFeature, ct)
        : null;

      if (scenario == null)
      {
        throw new NotFoundException(scenarioCode, "Scenario");
      }
      var result = ScenarioMapper.MapToScenarioDTO(scenario);
      if (inputFeature != null)
      {
        result.InputFeatureSummary = FeatureMapper.MapToFeatureSummaryDTO(inputFeature);
      }
      
      if (outputFeature != null)
      {
        result.OutputFeatureSummary = FeatureMapper.MapToFeatureSummaryDTO(outputFeature);
      }
      return result;
    }
  }

  /// <summary>
  /// Ověří existenci scénáře podle Id bez výjimek.
  /// </summary>
  public async Task<bool> ExistsByIdAsync(Datasource dataSource, Guid scenarioId, CancellationToken ct)
  {
    if (dataSource == Datasource.AVAPlace)
    {
      // AVAPlace nevrací snadný exists; použijeme try/catch nad GetScenario
      try
      {
        await integrationDataProvider.GetScenario(scenarioId);
        return true;
      }
      catch (NotFoundException)
      {
        return false;
      }
    }
    else
    {
      return await databaseContext.Scenarios.AnyAsync(s => s.Id == scenarioId, ct);
    }
  }

  /// <summary>
  /// Ověří existenci scénáře podle kódu bez výjimek.
  /// </summary>
  public async Task<bool> ExistsByCodeAsync(Datasource dataSource, string scenarioCode, CancellationToken ct)
  {
    if (dataSource == Datasource.AVAPlace)
    {
      // AVAPlace nevrací snadný exists; použijeme try/catch nad GetScenario
      try
      {
        await integrationDataProvider.GetScenario(scenarioCode);
        return true;
      }
      catch (NotFoundException)
      {
        return false;
      }
    }
    else
    {
      return await databaseContext.Scenarios.AnyAsync(s => s.Code == scenarioCode, ct);
    }
  }

  /// <inheritdoc />
  public void InvalidateCache(Datasource datasource)
  {
    _memoryCache.Remove(getChacheKey(datasource));
  }
}
