using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.DeploymentAggregate;

namespace AVAIntegrationModeler.UseCases.Deployments.Create;

/// <summary>
/// Handler pro příkaz vytvoření nasazení.
/// </summary>
public class CreateDeploymentHandler(
  IRepository<Deployment> repository,
  IDeploymentsQueryService queryService
) : ICommandHandler<CreateDeploymentCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateDeploymentCommand request, CancellationToken cancellationToken)
  {
    Deployment deployment;
    try
    {
      deployment = new Deployment(
        request.Deployment.Id == Guid.Empty ? Guid.NewGuid() : request.Deployment.Id,
        request.Deployment.Code);
      deployment.SetName(request.Deployment.Name);
      foreach (var id in request.Deployment.DataModelIds)
        deployment.AddDataModel(id);
    }
    catch (ArgumentException ex)
    {
      return Result<Guid>.Invalid(new ValidationError
      {
        Identifier = ex.ParamName ?? "Deployment",
        ErrorMessage = ex.Message
      });
    }

    var exists = await queryService.ExistsByCodeAsync(deployment.Code, cancellationToken);
    if (exists)
      return Result<Guid>.Conflict($"Nasazení s kódem '{deployment.Code}' již existuje.");

    Deployment? created;
    try
    {
      created = await repository.AddAsync(deployment, cancellationToken);
      queryService.InvalidateCache(Datasource.Database);
    }
    catch (Exception ex)
    {
      return Result<Guid>.Error($"Nasazení se nepodařilo vytvořit: {ex.Message}");
    }

    if (created is null)
      return Result<Guid>.Error("Nasazení nebylo vytvořeno.");

    return Result<Guid>.Success(created.Id);
  }
}
