using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.Extensions.Caching.Memory;

namespace AVAIntegrationModeler.Infrastructure.Data.Queries;

public class ListFeaturesQueryService(
  //AppDbContext _db, 
  IIntegrationDataProvider integrationDataProvider,
  IMemoryCache memoryCache) : AVAIntegrationModeler.UseCases.Features.List.IListFeaturesQueryService
{
  private const string primaryKeyName = "ScenarioListQuery";
  private readonly IMemoryCache _memoryCache = memoryCache;

  private string createChacheKey(Datasource datasource) => $"{primaryKeyName}-{datasource}";
  public async Task<IEnumerable<FeatureDTO>> ListAsync(Datasource datasouce) 
  {
    datasouce.GetHashCode();
    List<FeatureDTO> result = new List<FeatureDTO>();
    var x = await _memoryCache.GetOrCreateAsync(
      createChacheKey(datasouce),
      async entry =>
      {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        entry.SlidingExpiration = TimeSpan.FromMinutes(2);
        entry.Priority = CacheItemPriority.Normal;
        List<FeatureDTO> result = new List<FeatureDTO>();
        if (datasouce == Datasource.AVAPlace)
        {
          result = (await integrationDataProvider.GetIntegrationFeaturesAsync(CancellationToken.None)).ToList();
        }
        else
        {
          throw new NotImplementedException();
          //result = await _db.Features
          //.Select(s => UseCases.Scenarios.Mapping.ScenarioMapper.MapToDTO(s))
          //.ToListAsync();
        }
        return result;
      });
    return await Task.FromResult(result);
    //return x!;
  }
    
}
