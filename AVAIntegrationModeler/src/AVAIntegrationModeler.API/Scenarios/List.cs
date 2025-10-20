using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Scenarios;
using AVAIntegrationModeler.UseCases.Scenarios.List;

namespace AVAIntegrationModeler.API.Scenarios;

/// <summary>
/// List all Scenarios
/// </summary>
/// <remarks>
/// List all contributors - returns a ContributorListResponse containing the Scenarios.
/// </remarks>
public class List(IMediator _mediator) : Endpoint<ScenarioListRequest,ScenarioListResponse>
{
  public override void Configure()
  {
    Get("/Scenarios");
    AllowAnonymous();
    Options(x => x.CacheOutput(p => p.Expire(TimeSpan.FromSeconds(5))));
  }

  public override async Task HandleAsync(ScenarioListRequest request, CancellationToken cancellationToken)
  {

    Result<IEnumerable<ScenarioDTO>> result = await _mediator.Send(new ListScenariosQuery(request.Datasource, null, null), cancellationToken);

    var result2 = await new ListScenariosQuery2(request.Datasource, null, null)
      .ExecuteAsync(cancellationToken);

    if (result.IsSuccess)
    {
      Response = new ScenarioListResponse
      {
        Scenarios = result.Value.ToList()
      };
    }
  }
}
