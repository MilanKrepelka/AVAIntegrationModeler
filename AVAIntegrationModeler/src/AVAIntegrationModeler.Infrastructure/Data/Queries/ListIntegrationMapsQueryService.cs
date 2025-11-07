using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Contributors;
using AVAIntegrationModeler.UseCases.Contributors.List;
using AVAIntegrationModeler.UseCases.IntegrationMaps.List;
using AVAIntegrationModeler.UseCases.Scenarios;
using AVAIntegrationModeler.UseCases.Scenarios.List;
using Microsoft.Extensions.Caching.Memory;

namespace AVAIntegrationModeler.Infrastructure.Data.Queries;

public class ListIntegrationMapsQueryService(
  AppDbContext _db, 
  IIntegrationDataProvider integrationDataProvider,
  IMemoryCache memoryCache) : IListIntegrationMapsQueryService
{
  private const string primaryKeyName = "IntegrationMapListQuery";
  private readonly IMemoryCache _memoryCache = memoryCache;

  private string createChacheKey(Datasource datasource) => $"{primaryKeyName}-{datasource}";
  public async Task<IEnumerable<ScenarioDTO>> ListAsync(Datasource datasouce) 
  {
    var methodResult = await _memoryCache.GetOrCreateAsync(
      createChacheKey(datasouce),
      async entry =>
      {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        entry.SlidingExpiration = TimeSpan.FromMinutes(2);
        entry.Priority = CacheItemPriority.Normal;
        
        List<ScenarioDTO> result;
        
        if (datasouce == Datasource.AVAPlace)
        {
          var scenariosTask = integrationDataProvider.GetScenariosAsync();
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
          // ✅ Explicitní join pomocí LINQ bez navigačních vlastností
          var scenariosTask = _db.Scenarios.ToListAsync();
          var allFeaturesTask = _db.Features.ToListAsync();
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
              ? new FeatureSummaryDTO 
                { 
                  Id = featureDict[s.InputFeature.Value].Id, 
                  Code = featureDict[s.InputFeature.Value].Code 
                }
              : null,
            OutputFeatureSummary = s.OutputFeature.HasValue && featureDict.ContainsKey(s.OutputFeature.Value)
              ? new FeatureSummaryDTO 
                { 
                  Id = featureDict[s.OutputFeature.Value].Id, 
                  Code = featureDict[s.OutputFeature.Value].Code 
                }
              : null
          }).ToList();
        }
        
        return result;
      });
    
    return methodResult!;
  }

  public async Task<IEnumerable<IntegrationMapSummaryDTO>> ListSummaryAsync(Datasource dataSource)
  {
    var methodResult = await _memoryCache.GetOrCreateAsync(
  createChacheKey(dataSource),
  async entry =>
  {
    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
    entry.SlidingExpiration = TimeSpan.FromMinutes(2);
    entry.Priority = CacheItemPriority.Normal;

    List<IntegrationMapSummaryDTO> result = new List<IntegrationMapSummaryDTO>();

    if (dataSource == Datasource.AVAPlace)
    {
      var integrationMapTask = integrationDataProvider.GetIntegrationMapSummaryAsync();
      var featuresTask = integrationDataProvider.GetFeaturesSummaryAsync();
      await Task.WhenAll(integrationMapTask, featuresTask);

      var integrationMap = integrationMapTask.Result.ToList();
      var features = featuresTask.Result.ToDictionary(f => f.Id);

      result = integrationMap;
    }
    else
    {
      // ✅ Explicitní join pomocí LINQ bez navigačních vlastností
      var scenariosTask = _db.Scenarios.ToListAsync();
      var allFeaturesTask = _db.Features.ToListAsync();
      await Task.WhenAll(scenariosTask, allFeaturesTask);

      var scenarios = scenariosTask.Result;
      var features = allFeaturesTask.Result;

      var featureDict = features.ToDictionary(f => f.Id);

      //result = scenarios.Select(s => new ScenarioDTO
      //{
      //  Id = s.Id,
      //  Code = s.Code,
      //  Name = s.Name,
      //  Description = s.Description,
      //  InputFeatureId = s.InputFeature,
      //  OutputFeatureId = s.OutputFeature,
      //  InputFeatureSummary = s.InputFeature.HasValue && featureDict.ContainsKey(s.InputFeature.Value)
      //    ? new FeatureSummaryDTO
      //    {
      //      Id = featureDict[s.InputFeature.Value].Id,
      //      Code = featureDict[s.InputFeature.Value].Code
      //    }
      //    : null,
      //  OutputFeatureSummary = s.OutputFeature.HasValue && featureDict.ContainsKey(s.OutputFeature.Value)
      //    ? new FeatureSummaryDTO
      //    {
      //      Id = featureDict[s.OutputFeature.Value].Id,
      //      Code = featureDict[s.OutputFeature.Value].Code
      //    }
      //    : null
      //}).ToList();
    }

    return result;
  });

    return methodResult!;

  }

  Task<IEnumerable<IntegrationMapDTO>> IListIntegrationMapsQueryService.ListAsync(Datasource dataSource)
  {
    throw new NotImplementedException();
  }
}
