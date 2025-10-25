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
    datasouce.GetHashCode();
    List<ScenarioDTO> result = new List<ScenarioDTO>();
    var x = await _memoryCache.GetOrCreateAsync(
      createChacheKey(datasouce),
      async entry =>
      {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        entry.SlidingExpiration = TimeSpan.FromMinutes(2);
        entry.Priority = CacheItemPriority.Normal;
        List<ScenarioDTO> result = new List<ScenarioDTO>();
        if (datasouce == Datasource.AVAPlace)
        {
          result = (await integrationDataProvider.GetIntegrationScenariosAsync(CancellationToken.None)).ToList();
        }
        else
        {
          result = await _db.Scenarios
          .Include(s => s.InputFeature)
          .Include(s => s.OutputFeature)
          .Select(s => UseCases.Scenarios.Mapping.ScenarioMapper.MapToDTO(s))
          .ToListAsync();
        }
        return result;
      });
    
    return x!;
  }
    
}
