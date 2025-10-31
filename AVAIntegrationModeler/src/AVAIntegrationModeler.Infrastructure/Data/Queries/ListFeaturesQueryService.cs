using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.DataModels.Mapping;
using AVAIntegrationModeler.UseCases.Features.Mapping;
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
          result = (await _db.Features
  .Include(f => f.IncludedFeatures)
  .Include(f => f.IncludedModels)
  .ToListAsync())
  .Select(s => UseCases.Features.Mapping.FeatureMapper.MapToFeatureSummaryDTO(s))
  .ToList();
        }
        return result;
      });

    return x ?? new List<FeatureSummaryDTO>();
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
          var featuresTask = integrationDataProvider.GetFeaturesAsync(CancellationToken.None);
          var modelsTask = integrationDataProvider.GetDataModelsSummaryAsync(CancellationToken.None);

          await Task.WhenAll(featuresTask, modelsTask);

          var featuresResult = featuresTask.Result.ToList();
          var modelsResult = modelsTask.Result.ToList();

          result = featuresResult.Select(f => new FeatureDTO
          {
            Id = f.Id,
            Code = f.Code,
            Name = f.Name,
            Description = f.Description,
            IncludedFeatures = f.IncludedFeatures?.Select(inc => new IncludedFeatureDTO()
            {
              Feature = FeatureMapper.MapToFeatureSummaryDTO(
                featuresResult.FirstOrDefault(item => item.Code == inc.Feature.Code) ?? FeatureDTO.Empty),
              ConsumeOnly = inc.ConsumeOnly
            }).ToList() ?? new List<IncludedFeatureDTO>(),

            IncludedModels = f.IncludedModels?.Select(inc => new IncludedDataModelDTO
            {
              DataModel = modelsResult.FirstOrDefault(m => m.Code == inc.DataModel.Code)
                          ?? DataModelSummaryDTO.Empty,
              ReadOnly = inc.ReadOnly
            }).ToList() ?? new List<IncludedDataModelDTO>()
          }).ToList();
          
        }
        else
        {
          List<FeatureSummaryDTO> features = _db.Features.Select(item=> FeatureMapper.MapToFeatureSummaryDTO(item)).ToList();
          List<DataModelSummaryDTO> models = _db.DataModels.Select(item=> DataModelMapper.MapToDataModelSummaryDTO(item)).ToList();

          result = (await _db.Features
          .Include(f => f.IncludedFeatures)
          .Include(f => f.IncludedModels)
          .ToListAsync())
          .Select(s => UseCases.Features.Mapping.FeatureMapper.MapToFeatureDTO(s, features, models))
          .ToList();
        }
        return result;
      });
    return x!;
  }

}
