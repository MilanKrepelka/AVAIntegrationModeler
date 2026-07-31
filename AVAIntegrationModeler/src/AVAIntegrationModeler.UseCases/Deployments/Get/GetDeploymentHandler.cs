using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Deployments.Get;

/// <summary>
/// Handler pro dotazy <see cref="GetDeploymentQuery"/> a <see cref="GetDeploymentByCodeQuery"/>.
/// </summary>
public class GetDeploymentHandler(IDeploymentsQueryService queryService)
  : IQueryHandler<GetDeploymentQuery, Result<DeploymentDTO>>,
    IQueryHandler<GetDeploymentByCodeQuery, Result<DeploymentDTO>>
{
  public async Task<Result<DeploymentDTO>> Handle(GetDeploymentQuery request, CancellationToken cancellationToken)
  {
    try
    {
      var dto = await queryService.GetDeployment(request.Id, cancellationToken);
      return Result<DeploymentDTO>.Success(dto);
    }
    catch (NotFoundException)
    {
      return Result<DeploymentDTO>.NotFound();
    }
  }

  public async Task<Result<DeploymentDTO>> Handle(GetDeploymentByCodeQuery request, CancellationToken cancellationToken)
  {
    try
    {
      var dto = await queryService.GetDeployment(request.Code, cancellationToken);
      return Result<DeploymentDTO>.Success(dto);
    }
    catch (NotFoundException)
    {
      return Result<DeploymentDTO>.NotFound();
    }
  }
}
