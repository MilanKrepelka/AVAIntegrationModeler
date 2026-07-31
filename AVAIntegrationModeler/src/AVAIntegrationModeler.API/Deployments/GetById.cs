using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Deployments.Get;

namespace AVAIntegrationModeler.API.Deployments;

/// <summary>
/// Vrátí nasazení podle identifikátoru.
/// </summary>
public class GetDeploymentByIdEndpoint(IMediator mediator) : Endpoint<GetDeploymentByIdRequest, DeploymentDTO>
{
  public override void Configure()
  {
    Get(GetDeploymentByIdRequest.Route);
    AllowAnonymous();
    Summary(s => s.Summary = "Vrátí nasazení podle identifikátoru.");
  }

  public override async Task HandleAsync(GetDeploymentByIdRequest request, CancellationToken cancellationToken)
  {
    var result = await mediator.Send(new GetDeploymentQuery(request.DeploymentId), cancellationToken);
    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }
    if (result.IsSuccess)
      Response = result.Value;
  }
}
