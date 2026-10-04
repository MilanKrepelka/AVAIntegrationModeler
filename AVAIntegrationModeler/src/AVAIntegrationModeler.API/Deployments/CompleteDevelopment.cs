using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Deployments.CompleteDevelopment;

namespace AVAIntegrationModeler.API.Deployments;

/// <summary>
/// Ukončí vývoj nasazení — nastaví datum posledního nasazení na aktuální čas a nasazení uloží.
/// </summary>
public class CompleteDeploymentDevelopmentEndpoint(IMediator mediator)
  : Endpoint<CompleteDeploymentDevelopmentRequest, DeploymentDTO>
{
  public override void Configure()
  {
    Post(CompleteDeploymentDevelopmentRequest.Route);
    AllowAnonymous();
    Summary(s => s.Summary = "Ukončí vývoj nasazení a nastaví datum posledního nasazení na aktuální čas.");
  }

  public override async Task HandleAsync(CompleteDeploymentDevelopmentRequest request, CancellationToken cancellationToken)
  {
    var result = await mediator.Send(new CompleteDeploymentDevelopmentCommand(request.DeploymentCode), cancellationToken);

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
