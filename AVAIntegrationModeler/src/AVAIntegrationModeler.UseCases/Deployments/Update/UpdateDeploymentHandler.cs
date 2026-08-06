using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain;
using AVAIntegrationModeler.Domain.DeploymentAggregate;
using AVAIntegrationModeler.Domain.DeploymentAggregate.Specifications;
using AVAIntegrationModeler.UseCases.Deployments.Mapping;

namespace AVAIntegrationModeler.UseCases.Deployments.Update;

/// <summary>
/// Handler pro příkaz aktualizace nasazení.
/// </summary>
public class UpdateDeploymentHandler(
  IDeploymentRepository repository,
  IDeploymentsQueryService queryService,
  IDomainEntityValidationService<Deployment> deploymentValidationService
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
      existing.SetTicket(request.Deployment.Ticket);
      existing.SetDescription(request.Deployment.Description);
    }
    catch (ArgumentException ex)
    {
      return Result<DeploymentDTO>.Invalid(new ValidationError
      {
        Identifier = ex.ParamName ?? "Deployment",
        ErrorMessage = ex.Message
      });
    }

    var validationResult = await deploymentValidationService.Validate(Datasource.Database, existing, cancellationToken);
    if (!validationResult.IsSuccess)
      return validationResult;

    var requestedIds = (request.Deployment.DataModelIds ?? []).ToHashSet();
    var existingIds = existing.DataModels.Select(dm => dm.DataModelId).ToHashSet();

    // Záznamy k odebrání: jsou v DB, ale nejsou v požadavku
    var toDelete = existing.DataModels
      .Where(dm => !requestedIds.Contains(dm.DataModelId))
      .ToList();

    // Záznamy k přidání: jsou v požadavku, ale nejsou v DB
    var toAddIds = requestedIds.Where(id => !existingIds.Contains(id)).ToList();

    foreach (var dm in toDelete)
      existing.RemoveDataModel(dm.DataModelId);
    foreach (var id in toAddIds)
      existing.AddDataModel(id);

    var toAdd = existing.DataModels
      .Where(dm => toAddIds.Contains(dm.DataModelId))
      .ToList();

    // DTO sestavíme před sync — EntityState.Unchanged v SyncDataModelsAndSaveAsync
    // resetuje EF snapshot a může vrátit skalární hodnoty na původní.
    var resultDto = new DeploymentDTO
    {
      Id = existing.Id,
      Code = request.Deployment.Code,
      Name = request.Deployment.Name,
      Ticket = request.Deployment.Ticket,
      Description = request.Deployment.Description,
      DataModelIds = requestedIds.ToList()
    };

    await repository.SyncDataModelsAndSaveAsync(existing, toDelete, toAdd, cancellationToken);
    queryService.InvalidateCache(Datasource.Database);

    return Result<DeploymentDTO>.Success(resultDto);
  }
}
