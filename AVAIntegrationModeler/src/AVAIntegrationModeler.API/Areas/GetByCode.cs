using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Areas.Get;

namespace AVAIntegrationModeler.API.Areas;

/// <summary>
/// Vrátí oblast podle kódu.
/// </summary>
public class GetAreaByCodeEndpoint(IMediator mediator) : Endpoint<GetAreaByCodeRequest, AreaDTO>
{
  public override void Configure()
  {
    Get(GetAreaByCodeRequest.Route);
    AllowAnonymous();
    Summary(s => s.Summary = "Vrátí oblast podle kódu.");
  }

  public override async Task HandleAsync(GetAreaByCodeRequest request, CancellationToken cancellationToken)
  {
    var result = await mediator.Send(new GetAreaByCodeQuery(request.Datasource, request.AreaCode), cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
      Response = result.Value;
  }
}
