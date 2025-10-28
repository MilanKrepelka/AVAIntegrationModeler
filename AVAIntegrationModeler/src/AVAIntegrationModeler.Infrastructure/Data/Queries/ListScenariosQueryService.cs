using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Contributors;
using AVAIntegrationModeler.UseCases.Contributors.List;
using AVAIntegrationModeler.UseCases.Scenarios;
using AVAIntegrationModeler.UseCases.Scenarios.List;
using Microsoft.Extensions.Caching.Memory;

namespace AVAIntegrationModeler.Infrastructure.Data.Queries;

public class ListScenariosQueryService(
  AppDbContext _db, 
  IIntegrationDataProvider integrationDataProvider,
  IMemoryCache memoryCache) : IListScenariosQueryService
{
  private const string primaryKeyName = "ScenarioListQuery";
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
          var scenarios = (await integrationDataProvider.GetScenariosAsync(CancellationToken.None)).ToList();
          var features = (await integrationDataProvider.GetFeaturesSummaryAsync(CancellationToken.None))
            .ToDictionary(f => f.Id);

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
          var scenarios = await _db.Scenarios.ToListAsync();
          var featureIds = scenarios
            .SelectMany(s => new[] { s.InputFeature, s.OutputFeature })
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();

          var features = await _db.Features
            .Where(f => featureIds.Contains(f.Id))
            .ToListAsync();

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
    
}
