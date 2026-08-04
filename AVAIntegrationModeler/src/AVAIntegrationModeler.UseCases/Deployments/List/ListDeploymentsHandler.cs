using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Deployments.List;

/// <summary>
/// Handler pro dotaz <see cref="ListDeploymentsQuery"/>.
/// </summary>
public class ListDeploymentsHandler(IDeploymentsQueryService queryService)
  : IQueryHandler<ListDeploymentsQuery, Result<IEnumerable<DeploymentDTO>>>
{
  public async Task<Result<IEnumerable<DeploymentDTO>>> Handle(
    ListDeploymentsQuery request, CancellationToken cancellationToken)
  {
    var items = await queryService.ListAsync();
    return Result<IEnumerable<DeploymentDTO>>.Success(items);
  }
}
