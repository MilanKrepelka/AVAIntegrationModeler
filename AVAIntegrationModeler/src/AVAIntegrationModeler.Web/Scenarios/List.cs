using AVAIntegrationModeler.UseCases.Scenarios;
using AVAIntegrationModeler.UseCases.Scenarios.List;

namespace AVAIntegrationModeler.API.Scenarios;

/// <summary>
/// List all Scenarios
/// </summary>
/// <remarks>
/// List all contributors - returns a ContributorListResponse containing the Scenarios.
/// </remarks>
public class List(IMediator _mediator) : EndpointWithoutRequest<ScenarioListResponse>
{
  public override void Configure()
  {
    Get("/Scenarios");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken cancellationToken)
  {
    Result<IEnumerable<ScenarioDTO>> result = await _mediator.Send(new ListScenariosQuery(null, null), cancellationToken);

    var result2 = await new ListScenariosQuery2(null, null)
      .ExecuteAsync(cancellationToken);

    if (result.IsSuccess)
    {
      Response = new ScenarioListResponse
      {
        Scenarios = result.Value.Select(scenatioDTO => new ScenarioRecord(scenatioDTO)).ToList()
      };
    }
  }
}
