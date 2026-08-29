using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.DeploymentAggregate;
using AVAIntegrationModeler.UseCases.Deployments;
using AVAIntegrationModeler.UseCases.Deployments.Mapping;
using Microsoft.Extensions.Caching.Memory;

namespace AVAIntegrationModeler.Infrastructure.Data.Queries;

/// <summary>
/// Implementace dotazovací služby pro nasazení.
/// </summary>
public class DeploymentsQueryService(
  AppDbContext databaseContext,
  IMemoryCache memoryCache) : IDeploymentsQueryService
{
  private const string CacheKey = "DeploymentListQuery";

  /// <inheritdoc />
  public async Task<IEnumerable<DeploymentDTO>> ListAsync()
  {
    var result = await memoryCache.GetOrCreateAsync(
      CacheKey,
      async entry =>
      {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        entry.SlidingExpiration = TimeSpan.FromMinutes(2);
        entry.Priority = CacheItemPriority.Normal;

        var deployments = await databaseContext.Deployments
          .Include(d => d.DataModels)
          .ToListAsync();
        return deployments.Select(d => DeploymentMapper.MapToDTO(d)!).ToList();
      });

    return result!;
  }

  /// <inheritdoc />
  public async Task<DeploymentDTO> GetDeployment(Guid id, CancellationToken cancellationToken)
  {
    var deployment = await databaseContext.Deployments
      .Include(d => d.DataModels)
      .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    if (deployment is null)
      throw new NotFoundException(id.ToString(), nameof(Deployment));
    return DeploymentMapper.MapToDTO(deployment)!;
  }

  /// <inheritdoc />
  public async Task<DeploymentDTO> GetDeployment(string code, CancellationToken cancellationToken)
  {
    var deployment = await databaseContext.Deployments
      .Include(d => d.DataModels)
      .FirstOrDefaultAsync(d => d.Code == code, cancellationToken);
    if (deployment is null)
      throw new NotFoundException(code, nameof(Deployment));
    return DeploymentMapper.MapToDTO(deployment)!;
  }

  /// <inheritdoc />
  public async Task<IEnumerable<DeploymentDTO>> ListRecentAsync(int count, CancellationToken cancellationToken = default)
  {
    var all = await ListAsync();
    return all.TakeLast(count);
  }

  /// <inheritdoc />
  public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken)
    => await databaseContext.Deployments.AnyAsync(d => d.Code == code, cancellationToken);

  /// <inheritdoc />
  public void InvalidateCache(Datasource datasource)
    => memoryCache.Remove(CacheKey);
}
