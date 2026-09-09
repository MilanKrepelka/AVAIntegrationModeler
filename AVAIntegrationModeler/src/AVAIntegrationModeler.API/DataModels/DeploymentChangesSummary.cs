using AVAIntegrationModeler.Contracts.DataModels;
using AVAIntegrationModeler.UseCases.DataModels.Compare;
using FastEndpoints;
using MediatR;

namespace AVAIntegrationModeler.API.DataModels;

/// <summary>
/// Endpoint pro souhrnné porovnání DataModelů nasazení mezi databází a AVAPlace.
/// DataModelIds jsou součástí těla požadavku.
/// </summary>
public class GetDeploymentChangesSummaryEndpoint(IMediator mediator)
  : Endpoint<GetDeploymentChangesSummaryRequest, GetDeploymentChangesSummaryResponse>
{
  /// <inheritdoc />
  public override void Configure()
  {
    Post("/DataModels/Deployment/changes/summary");
    AllowAnonymous();
    Summary(s => s.Summary = "Souhrnné porovnání DataModelů nasazení mezi databází a AVAPlace.");
  }

  /// <inheritdoc />
  public override async Task HandleAsync(GetDeploymentChangesSummaryRequest request, CancellationToken ct)
  {
    var result = await mediator.Send(
      new GetDeploymentChangesSummaryQuery(request.DeploymentCode, request.DeploymentName, request.DataModelIds), ct);

    if (result.IsSuccess)
    {
      Response = new GetDeploymentChangesSummaryResponse { Summary = result.Value };
      return;
    }
    await SendErrorsAsync(cancellation: ct);
  }
}
