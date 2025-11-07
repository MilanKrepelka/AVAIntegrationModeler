using AVAIntegrationModeler.API.IntegrationMaps;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.IntegrationMaps;
using AVAIntegrationModeler.UseCases.IntegrationMaps.List;

namespace AVAIntegrationModeler.API.IntegrationMaps;

/// <summary>
/// List all Integration Maps
/// </summary>
/// <remarks>
/// List all Integration Maps - returns a IntegrationMapsListResponse containing the Integration Maps.
/// </remarks>
public class List(IMediator _mediator) : Endpoint<IntegrationMapListRequest, IntegrationMapListResponse>
{
  public override void Configure()
  {
    Get("/IntegrationMaps");
    AllowAnonymous();
    Options(x => x.CacheOutput(p => p.Expire(TimeSpan.FromSeconds(5))));
  }

  public override async Task HandleAsync(IntegrationMapListRequest request, CancellationToken cancellationToken)
  {

    Result<IEnumerable<IntegrationMapSummaryDTO>> result = await _mediator.Send(new ListIntegrationMapsQuery(request.Datasource, null, null), cancellationToken);

    var result2 = await new ListIntegrationMapsQuery2(request.Datasource, null, null)
      .ExecuteAsync(cancellationToken);

    if (result.IsSuccess)
    {
      Response = new IntegrationMapListResponse
      {
        IntegrationMaps = result.Value.ToList()
      };
    }
  }
}
