using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.AreaAggregate;
using AVAIntegrationModeler.UseCases.Areas;
using AVAIntegrationModeler.UseCases.Areas.Mapping;
using Microsoft.Extensions.Caching.Memory;

namespace AVAIntegrationModeler.Infrastructure.Data.Queries;

/// <summary>
/// Implementace dotazovací služby pro oblasti.
/// </summary>
/// <param name="databaseContext"><see cref="AppDbContext"/></param>
/// <param name="memoryCache"><see cref="IMemoryCache"/></param>
public class AreasQueryService(
  AppDbContext databaseContext,
  IMemoryCache memoryCache) : IAreasQueryService
{
  private const string PrimaryKeyName = "AreaListQuery";

  private string GetCacheKey(Datasource datasource) => $"{PrimaryKeyName}-{datasource}";

  /// <inheritdoc />
  public async Task<IEnumerable<AreaDTO>> ListAsync(Datasource datasource)
  {
    var result = await memoryCache.GetOrCreateAsync(
      GetCacheKey(datasource),
      async entry =>
      {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        entry.SlidingExpiration = TimeSpan.FromMinutes(2);
        entry.Priority = CacheItemPriority.Normal;

        var areas = await databaseContext.Areas.ToListAsync();
        return areas.Select(a => AreaMapper.MapToAreaDTO(a)!).ToList();
      });

    return result!;
  }

  /// <inheritdoc />
  public async Task<AreaDTO> GetArea(Datasource datasource, Guid areaId, CancellationToken cancellationToken)
  {
    var area = await databaseContext.Areas.FirstOrDefaultAsync(a => a.Id == areaId, cancellationToken);
    if (area == null)
      throw new NotFoundException(areaId.ToString(), nameof(Area));

    return AreaMapper.MapToAreaDTO(area)!;
  }

  /// <inheritdoc />
  public async Task<AreaDTO> GetArea(Datasource datasource, string areaCode, CancellationToken cancellationToken)
  {
    var area = await databaseContext.Areas.FirstOrDefaultAsync(a => a.Code == areaCode, cancellationToken);
    if (area == null)
      throw new NotFoundException(areaCode, nameof(Area));

    return AreaMapper.MapToAreaDTO(area)!;
  }

  /// <inheritdoc />
  public async Task<bool> ExistsByIdAsync(Datasource datasource, Guid areaId, CancellationToken cancellationToken)
    => await databaseContext.Areas.AnyAsync(a => a.Id == areaId, cancellationToken);

  /// <inheritdoc />
  public async Task<bool> ExistsByCodeAsync(Datasource datasource, string areaCode, CancellationToken cancellationToken)
    => await databaseContext.Areas.AnyAsync(a => a.Code == areaCode, cancellationToken);

  /// <inheritdoc />
  public void InvalidateCache(Datasource datasource)
    => memoryCache.Remove(GetCacheKey(datasource));
}
