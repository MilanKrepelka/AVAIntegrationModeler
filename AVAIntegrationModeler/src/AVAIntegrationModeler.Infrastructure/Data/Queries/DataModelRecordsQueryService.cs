using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.DataModelRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace AVAIntegrationModeler.Infrastructure.Data.Queries;

public class DataModelRecordsQueryService(
  AppDbContext _db,
  IIntegrationDataProvider integrationDataProvider,
  IMemoryCache memoryCache) : IDataModelRecordQueryService
{
  private const string CacheKeyPrefix = "DataModelRecordsQuery";

  private static string GetCacheKey(Datasource datasource, string method, Guid? modelId = null)
    => $"{CacheKeyPrefix}-{datasource}-{method}-{modelId}";

  public async Task<IEnumerable<DataModelRecordDTO>> ListAsync(
    Datasource datasource,
    Guid? modelId = null,
    int? skip = null,
    int? take = null,
    CancellationToken cancellationToken = default)
  {
    var cacheKey = GetCacheKey(datasource, nameof(ListAsync), modelId);
    var result = await memoryCache.GetOrCreateAsync(cacheKey, async entry =>
    {
      entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
      entry.SlidingExpiration = TimeSpan.FromMinutes(2);
      entry.Priority = CacheItemPriority.Normal;

      if (datasource == Datasource.AVAPlace)
      {
        return (await integrationDataProvider.GetDataModelRecordsAsync(modelId ?? Guid.Empty, cancellationToken)).ToList();
      }

      var query = _db.Set<Domain.DataModelRecordAggregate.DataModelRecord>()
        .Include(r => r.Fields)
        .AsQueryable();

      if (modelId.HasValue)
        query = query.Where(r => r.ModelId == modelId.Value);

      if (skip.HasValue) query = query.Skip(skip.Value);
      if (take.HasValue) query = query.Take(take.Value);

      return (await query.ToListAsync(cancellationToken))
        .Select(DataModelRecordMapper.MapToDTO)
        .ToList();
    });

    return result ?? Enumerable.Empty<DataModelRecordDTO>();
  }

  public async Task<DataModelRecordDTO?> GetByIdAsync(
    Datasource datasource,
    Guid id,
    CancellationToken cancellationToken = default)
  {
    if (datasource == Datasource.AVAPlace)
      return null;

    var record = await _db.Set<Domain.DataModelRecordAggregate.DataModelRecord>()
      .Include(r => r.Fields)
      .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    return record is null ? null : DataModelRecordMapper.MapToDTO(record);
  }

  public void InvalidateCache(Datasource datasource)
  {
    memoryCache.Remove(GetCacheKey(datasource, nameof(ListAsync)));
    memoryCache.Remove(GetCacheKey(datasource, nameof(ListAsync), null));
  }
}
