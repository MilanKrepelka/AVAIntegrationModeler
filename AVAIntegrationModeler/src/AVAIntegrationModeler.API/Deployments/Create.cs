using AVAIntegrationModeler.UseCases.Deployments.Create;

namespace AVAIntegrationModeler.API.Deployments;

/// <summary>
/// Vytvoří nové nasazení.
/// </summary>
public class CreateDeploymentEndpoint(IMediator mediator) : Endpoint<CreateDeploymentRequest, Guid>
{
  public override void Configure()
  {
    Post(CreateDeploymentRequest.Route);
    AllowAnonymous();
    Summary(s => s.Summary = "Vytvoří nové nasazení.");
  }

  public override async Task HandleAsync(CreateDeploymentRequest request, CancellationToken cancellationToken)
  {
    var result = await mediator.Send(new CreateDeploymentCommand(request.Deployment), cancellationToken);

    if (result.IsSuccess)
    {
      Response = result.Value;
      await SendCreatedAtAsync<GetDeploymentByIdEndpoint>(
        new { deploymentId = result.Value },
        Response,
        cancellation: cancellationToken);
      return;
    }

    if (result.Status == ResultStatus.Invalid)
    {
      foreach (var error in result.ValidationErrors)
        AddError(error.Identifier ?? "Deployment", error.ErrorMessage);
      await SendErrorsAsync(400, cancellationToken);
      return;
    }

    if (result.Status == ResultStatus.Conflict)
    {
      AddError(string.Join("; ", result.Errors));
      await SendErrorsAsync(409, cancellationToken);
      return;
    }

    AddError(string.Join("; ", result.Errors));
    await SendErrorsAsync(500, cancellationToken);
  }
}
