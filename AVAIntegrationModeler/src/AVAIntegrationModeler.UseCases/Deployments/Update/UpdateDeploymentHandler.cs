using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.DeploymentAggregate;
using AVAIntegrationModeler.Domain.DeploymentAggregate.Specifications;
using AVAIntegrationModeler.UseCases.Deployments.Mapping;

namespace AVAIntegrationModeler.UseCases.Deployments.Update;

/// <summary>
/// Handler pro příkaz aktualizace nasazení.
/// </summary>
public class UpdateDeploymentHandler(
  IRepository<Deployment> repository,
  IDeploymentsQueryService queryService
) : ICommandHandler<UpdateDeploymentCommand, Result<DeploymentDTO>>
{
  public async Task<Result<DeploymentDTO>> Handle(UpdateDeploymentCommand request, CancellationToken cancellationToken)
  {
    var existing = await repository.FirstOrDefaultAsync(
      new DeploymentByIdSpec(request.Deployment.Id), cancellationToken);
    if (existing is null)
      return Result<DeploymentDTO>.NotFound();

    try
    {
      existing.SetCode(request.Deployment.Code);
      existing.SetName(request.Deployment.Name);
    }
    catch (ArgumentException ex)
    {
      return Result<DeploymentDTO>.Invalid(new ValidationError
      {
        Identifier = ex.ParamName ?? "Deployment",
        ErrorMessage = ex.Message
      });
    }

    // Synchronizace DataModels — odebrat vše, přidat nové
    foreach (var dm in existing.DataModels.ToList())
      existing.RemoveDataModel(dm.DataModelId);
    foreach (var id in request.Deployment.DataModelIds)
      existing.AddDataModel(id);

    await repository.UpdateAsync(existing, cancellationToken);
    queryService.InvalidateCache(Datasource.Database);

    return Result<DeploymentDTO>.Success(DeploymentMapper.MapToDTO(existing)!);
  }
}
