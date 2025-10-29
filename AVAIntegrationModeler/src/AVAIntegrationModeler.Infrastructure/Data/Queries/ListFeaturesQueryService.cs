using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Scenarios.Mapping;
using Microsoft.Extensions.Caching.Memory;

namespace AVAIntegrationModeler.Infrastructure.Data.Queries;

public class ListFeaturesQueryService(
  AppDbContext _db, 
  IIntegrationDataProvider integrationDataProvider,
  IMemoryCache memoryCache) : AVAIntegrationModeler.UseCases.Features.List.IListFeaturesQueryService
{

  private const string primaryKeyName = "FeatureListQuery";
  private readonly IMemoryCache _memoryCache = memoryCache;
  private string createChacheKey(Datasource datasource, string methodName) => $"{primaryKeyName}-{datasource}-{methodName}";
  
  /// <inheritdoc/>
  public async Task<IEnumerable<FeatureSummaryDTO>> ListSummaryAsync(Datasource datasouce)
  {
    datasouce.GetHashCode();
    List<FeatureSummaryDTO> result = new List<FeatureSummaryDTO>();
    var x = await _memoryCache.GetOrCreateAsync(
      createChacheKey(datasouce, nameof(ListSummaryAsync)),
      async entry =>
      {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        entry.SlidingExpiration = TimeSpan.FromMinutes(2);
        entry.Priority = CacheItemPriority.Normal;
        List<FeatureSummaryDTO> result = new List<FeatureSummaryDTO>();
        if (datasouce == Datasource.AVAPlace)
        {
          result = (await integrationDataProvider.GetFeaturesSummaryAsync(CancellationToken.None)).ToList();
        }
        else
        {
          //throw new NotImplementedException();
          //result = await _db.Features
          //.Select(s => UseCases.Scenarios.Mapping.ScenarioMapper.MapToDTO(s))
          //.ToListAsync();
        }
        return result;
      });
    return await Task.FromResult(result);
    //return x!;
  }

  /// <inheritdoc/>
  public async Task<IEnumerable<FeatureDTO>> ListAsync(Datasource datasouce)
  {
    datasouce.GetHashCode();
    List<FeatureDTO> result = new List<FeatureDTO>();
    var x = await _memoryCache.GetOrCreateAsync(
      createChacheKey(datasouce, nameof(ListAsync)),
      async entry =>
      {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        entry.SlidingExpiration = TimeSpan.FromMinutes(2);
        entry.Priority = CacheItemPriority.Normal;
        List<FeatureDTO> result = new List<FeatureDTO>();
        if (datasouce == Datasource.AVAPlace)
        {
          var callResult = (await integrationDataProvider.GetFeaturesAsync(CancellationToken.None)).ToList();
          result = callResult.Select(f => new FeatureDTO
          {
            Id = f.Id,
            Code = f.Code,
            Name = f.Name,
            Description = f.Description
          }).ToList();
        }
        else
        {
          result = await _db.Features
          .Select(s => UseCases.Features.Mapping.FeatureMapper.MapToFeatureDTO(s))
          .ToListAsync();
        }
        return result;
      });
    return await Task.FromResult(result);
    //return x!;
  }

}
