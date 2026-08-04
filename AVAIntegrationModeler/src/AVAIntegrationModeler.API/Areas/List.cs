using AVAIntegrationModeler.Contracts.Areas;
using AVAIntegrationModeler.UseCases.Areas.List;

namespace AVAIntegrationModeler.API.Areas;

/// <summary>
/// Vrátí seznam oblastí.
/// </summary>
public class ListAreasEndpoint(IMediator mediator) : Endpoint<AreaListRequest, AreaListResponse>
{
  public override void Configure()
  {
    Get("/Areas");
    AllowAnonymous();
    Summary(s => s.Summary = "Vrátí seznam všech oblastí.");
  }

  public override async Task HandleAsync(AreaListRequest request, CancellationToken cancellationToken)
  {
    var result = await mediator.Send(new ListAreasQuery(request.Datasource), cancellationToken);

    if (result.IsSuccess)
      Response = new AreaListResponse { Areas = result.Value.ToList() };
  }
}
