using AVAIntegrationModeler.UseCases.Deployments.Delete;

namespace AVAIntegrationModeler.API.Deployments;

/// <summary>
/// Smaže nasazení.
/// </summary>
public class DeleteDeploymentEndpoint(IMediator mediator) : Endpoint<DeleteDeploymentRequest, bool>
{
  public override void Configure()
  {
    Delete(DeleteDeploymentRequest.Route);
    AllowAnonymous();
    Summary(s => s.Summary = "Smaže nasazení podle kódu.");
  }

  public override async Task HandleAsync(DeleteDeploymentRequest request, CancellationToken cancellationToken)
  {
    var result = await mediator.Send(new DeleteDeploymentCommand(request.DeploymentCode), cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      await SendNoContentAsync(cancellationToken);
      return;
    }

    await SendErrorsAsync(400, cancellationToken);
  }
}
