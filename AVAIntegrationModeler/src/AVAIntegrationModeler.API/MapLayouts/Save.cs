using AVAIntegrationModeler.UseCases.MapLayouts.Save;

namespace AVAIntegrationModeler.API.MapLayouts;

/// <summary>
/// Uloží JSON diagramu pojmenované mapy (upsert).
/// </summary>
public class SaveMapLayoutEndpoint(IMediator mediator) : Endpoint<SaveMapLayoutRequest>
{
  public override void Configure()
  {
    Put(SaveMapLayoutRequest.Route);
    AllowAnonymous();
    Summary(s => s.Summary = "Uloží JSON diagramu pojmenované mapy (upsert podle klíče).");
  }

  public override async Task HandleAsync(SaveMapLayoutRequest request, CancellationToken cancellationToken)
  {
    var result = await mediator.Send(new SaveMapLayoutCommand(request.Key, request.DiagramJson), cancellationToken);

    if (result.IsSuccess)
      await SendNoContentAsync(cancellationToken);
    else
      await SendErrorsAsync(500, cancellationToken);
  }
}
