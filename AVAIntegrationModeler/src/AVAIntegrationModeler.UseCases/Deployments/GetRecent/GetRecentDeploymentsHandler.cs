using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Deployments.GetRecent;

/// <summary>
/// Handler pro dotaz <see cref="GetRecentDeploymentsQuery"/>.
/// </summary>
public class GetRecentDeploymentsHandler(IDeploymentsQueryService queryService)
  : IQueryHandler<GetRecentDeploymentsQuery, Result<IEnumerable<DeploymentDTO>>>
{
  /// <inheritdoc />
  public async Task<Result<IEnumerable<DeploymentDTO>>> Handle(
    GetRecentDeploymentsQuery request, CancellationToken cancellationToken)
  {
    var items = await queryService.ListRecentAsync(request.Count, cancellationToken);
    return Result<IEnumerable<DeploymentDTO>>.Success(items);
  }
}
