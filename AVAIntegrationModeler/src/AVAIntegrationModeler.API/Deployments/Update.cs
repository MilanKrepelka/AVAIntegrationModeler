using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Deployments.Update;

namespace AVAIntegrationModeler.API.Deployments;

/// <summary>
/// Aktualizuje existující nasazení.
/// </summary>
public class UpdateDeploymentEndpoint(IMediator mediator) : Endpoint<UpdateDeploymentRequest, DeploymentDTO>
{
  public override void Configure()
  {
    Put(UpdateDeploymentRequest.Route);
    AllowAnonymous();
    Summary(s => s.Summary = "Aktualizuje existující nasazení.");
  }

  public override async Task HandleAsync(UpdateDeploymentRequest request, CancellationToken cancellationToken)
  {
    var result = await mediator.Send(new UpdateDeploymentCommand(request.Deployment), cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.Status == ResultStatus.Invalid)
    {
      foreach (var error in result.ValidationErrors)
        AddError(error.Identifier ?? "Deployment", error.ErrorMessage);
      await SendErrorsAsync(400, cancellationToken);
      return;
    }

    await SendOkAsync(result.Value, cancellationToken);
  }
}
