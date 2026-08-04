using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Deployments.Get;

namespace AVAIntegrationModeler.API.Deployments;

/// <summary>
/// Vrátí nasazení podle kódu.
/// </summary>
public class GetDeploymentByCodeEndpoint(IMediator mediator) : Endpoint<GetDeploymentByCodeRequest, DeploymentDTO>
{
  public override void Configure()
  {
    Get(GetDeploymentByCodeRequest.Route);
    AllowAnonymous();
    Summary(s => s.Summary = "Vrátí nasazení podle kódu.");
  }

  public override async Task HandleAsync(GetDeploymentByCodeRequest request, CancellationToken cancellationToken)
  {
    var result = await mediator.Send(new GetDeploymentByCodeQuery(request.DeploymentCode), cancellationToken);
    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }
    if (result.IsSuccess)
      Response = result.Value;
  }
}
