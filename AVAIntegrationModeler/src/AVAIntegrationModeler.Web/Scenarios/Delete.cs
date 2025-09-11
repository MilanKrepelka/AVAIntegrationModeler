using AVAIntegrationModeler.UseCases.Contributors.Delete;
using AVAIntegrationModeler.UseCases.Scenarios.Delete;

namespace AVAIntegrationModeler.API.Scenarios;

/// <summary>
/// Delete a Scenario.
/// </summary>
/// <remarks>
/// Delete a Contributor by providing a valid integer id.
/// </remarks>
public class Delete(IMediator _mediator)
  : Endpoint<DeleteScenarioRequest>
{
  public override void Configure()
  {
    Delete(DeleteScenarioRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(
    DeleteScenarioRequest request,
    CancellationToken cancellationToken)
  {
    var command = new DeleteScenarioCommand(request.ScenarioId);

    var result = await _mediator.Send(command, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      await SendNoContentAsync(cancellationToken);
    };
    // TODO: Handle other issues as needed
  }
}
