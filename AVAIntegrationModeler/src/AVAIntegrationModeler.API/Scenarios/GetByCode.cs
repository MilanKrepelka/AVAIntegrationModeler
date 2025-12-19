using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Contributors.Get;
using AVAIntegrationModeler.UseCases.Scenarios.Get;

namespace AVAIntegrationModeler.API.Scenarios;

/// <summary>
/// Get a Scenario by Scenario code.
/// </summary>
public class GetByCode(IMediator _mediator)
  : Endpoint<GetScenarioByCodeRequest, ScenarioDTO>
{
  public override void Configure()
  {
    Get(GetScenarioByCodeRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetScenarioByCodeRequest request,
    CancellationToken cancellationToken)
  {
    var query = new GetScenarioByCodeQuery(request.Datasource, request.ScenarioCode);

    var result = await _mediator.Send(query, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
     Response = result.Value;
    }
  }
}
