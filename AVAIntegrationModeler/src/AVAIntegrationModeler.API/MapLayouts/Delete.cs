using AVAIntegrationModeler.UseCases.MapLayouts.Delete;

namespace AVAIntegrationModeler.API.MapLayouts;

/// <summary>
/// Smaže uložený layout pojmenované mapy.
/// </summary>
public class DeleteMapLayoutEndpoint(IMediator mediator) : Endpoint<DeleteMapLayoutRequest>
{
  public override void Configure()
  {
    Delete(DeleteMapLayoutRequest.Route);
    AllowAnonymous();
    Summary(s => s.Summary = "Smaže uložený JSON diagramu pojmenované mapy.");
  }

  public override async Task HandleAsync(DeleteMapLayoutRequest request, CancellationToken cancellationToken)
  {
    var result = await mediator.Send(new DeleteMapLayoutCommand(request.Key), cancellationToken);

    if (result.IsSuccess)
      await SendNoContentAsync(cancellationToken);
    else if (result.Status == Ardalis.Result.ResultStatus.NotFound)
      await SendNotFoundAsync(cancellationToken);
    else
      await SendErrorsAsync(500, cancellationToken);
  }
}
