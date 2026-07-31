using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.DeploymentAggregate;
using AVAIntegrationModeler.Domain.DeploymentAggregate.Specifications;

namespace AVAIntegrationModeler.UseCases.Deployments.Delete;

/// <summary>
/// Handler pro příkaz smazání nasazení.
/// </summary>
public class DeleteDeploymentHandler(
  IRepository<Deployment> repository,
  IDeploymentsQueryService queryService
) : ICommandHandler<DeleteDeploymentCommand, Result>
{
  public async Task<Result> Handle(DeleteDeploymentCommand request, CancellationToken cancellationToken)
  {
    var deployment = await repository.FirstOrDefaultAsync(
      new DeploymentByCodeSpec(request.Code), cancellationToken);
    if (deployment is null)
      return Result.NotFound();

    await repository.DeleteAsync(deployment, cancellationToken);
    queryService.InvalidateCache(Datasource.Database);
    return Result.Success();
  }
}
