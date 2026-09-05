using AVAIntegrationModeler.UseCases.Areas.SaveMap;

namespace AVAIntegrationModeler.API.Areas;

/// <summary>
/// Uloží JSON diagramu mapy pro danou oblast.
/// </summary>
public class SaveAreaMapEndpoint(IMediator mediator) : Endpoint<SaveAreaMapRequest>
{
  public override void Configure()
  {
    Put(SaveAreaMapRequest.Route);
    AllowAnonymous();
    Summary(s => s.Summary = "Uloží JSON diagramu mapy oblasti.");
  }

  public override async Task HandleAsync(SaveAreaMapRequest request, CancellationToken cancellationToken)
  {
    var result = await mediator.Send(new SaveAreaMapCommand(request.AreaId, request.DiagramJson), cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    await SendNoContentAsync(cancellationToken);
  }
}
