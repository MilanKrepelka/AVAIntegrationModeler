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
  }

  public override async Task HandleAsync(ScenarioListRequest request, CancellationToken cancellationToken)
  {
    Result<IEnumerable<ScenarioDTO>> result = await _mediator.Send(new ListScenariosQuery(request.Datasouce, null, null), cancellationToken);

    var result2 = await new ListScenariosQuery2(request.Datasouce, null, null)
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
