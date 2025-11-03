using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.DataModels.Mapping;
using AVAIntegrationModeler.UseCases.Features.Mapping;
using AVAIntegrationModeler.UseCases.Scenarios.Mapping;
using Microsoft.Extensions.Caching.Memory;

namespace AVAIntegrationModeler.Infrastructure.Data.Queries;

public class ListDataModelsQueryService(
  AppDbContext _db,
  IIntegrationDataProvider integrationDataProvider,
  IMemoryCache memoryCache) : AVAIntegrationModeler.UseCases.DataModels.List.IListDataModelQueryService
{

  private const string PrimaryKeyName = "DataModelsListQuery";
  private readonly IMemoryCache _memoryCache = memoryCache;
  private static string createChacheKey(Datasource datasource, string methodName) => $"{PrimaryKeyName}-{datasource}-{methodName}";

  /// <inheritdoc/>
  public Task<IEnumerable<DataModelSummaryDTO>> ListSummaryAsync(Datasource datasouce)
  {
    throw new NotImplementedException();
    //  datasouce.GetHashCode();

    //  var x = await _memoryCache.GetOrCreateAsync(

    //    createChacheKey(datasouce, nameof(ListSummaryAsync)),
    //    async entry =>
    //    {

    //      entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
    //      entry.SlidingExpiration = TimeSpan.FromMinutes(2);
    //      entry.Priority = CacheItemPriority.Normal;
    //      List<FeatureSummaryDTO> result = new List<FeatureSummaryDTO>();
    //      if (datasouce == Datasource.AVAPlace)
    //      {
    //        result = (await integrationDataProvider.GetFeaturesSummaryAsync(CancellationToken.None)).ToList();
    //      }
    //      else
    //      {
    //        result = (await _db.Features
    //.Include(f => f.IncludedFeatures)
    //.Include(f => f.IncludedModels)
    //.ToListAsync())
    //.Select(s => UseCases.Features.Mapping.FeatureMapper.MapToFeatureSummaryDTO(s))
    //.ToList();
    //      }
    //      return result;
    //    });

    //  return x ?? new List<DataModelSummaryDTO>();
  }

  /// <inheritdoc/>
  public async Task<IEnumerable<DataModelDTO>> ListAsync(Datasource datasouce)
  {

    datasouce.GetHashCode();
    List<DataModelDTO> result = new List<DataModelDTO>();
    var x = await _memoryCache.GetOrCreateAsync(
      createChacheKey(datasouce, nameof(ListAsync)),
      async entry =>
      {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        entry.SlidingExpiration = TimeSpan.FromMinutes(2);
        entry.Priority = CacheItemPriority.Normal;
        List<DataModelDTO> result = new List<DataModelDTO>();
        if (datasouce == Datasource.AVAPlace)
        {
          var modelsTask = integrationDataProvider.GetDataModelsAsync(CancellationToken.None);
          await Task.WhenAll(modelsTask);
          result = modelsTask.Result.ToList();
        }
        else
        {
          
          result = (await _db.DataModels
          .Include(f => f.Fields)
          .ToListAsync())
          .Select(s => UseCases.DataModels.Mapping.DataModelMapper.MapToDataModelDTO(s))
          .ToList();
        }
        return result;
      });
    return x!;
  }

}
