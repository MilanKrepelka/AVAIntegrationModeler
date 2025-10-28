﻿using AVAIntegrationModeler.AVAPlace;
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
  public async Task<IEnumerable<FeatureSummaryDTO>> ListSummaryAsync(Datasource datasouce) 
  {
    datasouce.GetHashCode();
    List<FeatureSummaryDTO> result = new List<FeatureSummaryDTO>();
    var x = await _memoryCache.GetOrCreateAsync(
      createChacheKey(datasouce),
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

  public Task<IEnumerable<FeatureDTO>> ListAsync(Datasource dataSource)
  {
    throw new NotImplementedException();
  }
}
