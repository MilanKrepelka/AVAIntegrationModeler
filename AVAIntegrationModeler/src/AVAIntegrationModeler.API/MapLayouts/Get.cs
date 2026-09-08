using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.MapLayouts.Get;

namespace AVAIntegrationModeler.API.MapLayouts;

/// <summary>
/// Vrátí MapLayout podle klíče.
/// </summary>
public class GetMapLayoutEndpoint(IMediator mediator) : Endpoint<GetMapLayoutRequest, MapLayoutDTO>
{
  public override void Configure()
  {
    Get(GetMapLayoutRequest.Route);
    AllowAnonymous();
    Summary(s => s.Summary = "Vrátí JSON diagramu pojmenované mapy.");
  }

  public override async Task HandleAsync(GetMapLayoutRequest request, CancellationToken cancellationToken)
  {
    var result = await mediator.Send(new GetMapLayoutQuery(request.Key), cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
      Response = result.Value;
  }
}
