using AVAIntegrationModeler.Contracts.Deployments;
using AVAIntegrationModeler.UseCases.Deployments.List;

namespace AVAIntegrationModeler.API.Deployments;

/// <summary>
/// Vrátí seznam nasazení.
/// </summary>
public class ListDeploymentsEndpoint(IMediator mediator) : EndpointWithoutRequest<DeploymentListResponse>
{
  public override void Configure()
  {
    Get("/Deployments");
    AllowAnonymous();
    Summary(s => s.Summary = "Vrátí seznam všech nasazení.");
  }

  public override async Task HandleAsync(CancellationToken cancellationToken)
  {
    var result = await mediator.Send(new ListDeploymentsQuery(), cancellationToken);
    if (result.IsSuccess)
      Response = new DeploymentListResponse { Deployments = result.Value.ToList() };
  }
}
