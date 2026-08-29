using AVAIntegrationModeler.Contracts.Deployments;
using AVAIntegrationModeler.UseCases.Deployments.GetRecent;

namespace AVAIntegrationModeler.API.Deployments;

/// <summary>
/// Vrátí seznam posledních nasazení.
/// </summary>
public class GetRecentDeploymentsEndpoint(IMediator mediator)
  : Endpoint<GetRecentDeploymentsRequest, DeploymentListResponse>
{
  /// <inheritdoc />
  public override void Configure()
  {
    Get("/Deployments/recent");
    AllowAnonymous();
    Summary(s => s.Summary = "Vrátí seznam posledních nasazení (výchozí počet: 5).");
  }

  /// <inheritdoc />
  public override async Task HandleAsync(GetRecentDeploymentsRequest req, CancellationToken ct)
  {
    var result = await mediator.Send(new GetRecentDeploymentsQuery(req.Count), ct);
    if (result.IsSuccess)
      Response = new DeploymentListResponse { Deployments = result.Value.ToList() };
  }
}
