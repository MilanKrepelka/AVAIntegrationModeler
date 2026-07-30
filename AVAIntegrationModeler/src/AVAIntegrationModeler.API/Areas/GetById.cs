using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Areas.Get;

namespace AVAIntegrationModeler.API.Areas;

/// <summary>
/// Vrátí oblast podle identifikátoru.
/// </summary>
public class GetAreaByIdEndpoint(IMediator mediator) : Endpoint<GetAreaByIdRequest, AreaDTO>
{
  public override void Configure()
  {
    Get(GetAreaByIdRequest.Route);
    AllowAnonymous();
    Summary(s => s.Summary = "Vrátí oblast podle identifikátoru.");
  }

  public override async Task HandleAsync(GetAreaByIdRequest request, CancellationToken cancellationToken)
  {
    var result = await mediator.Send(new GetAreaQuery(request.Datasource, request.AreaId), cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
      Response = result.Value;
  }
}
