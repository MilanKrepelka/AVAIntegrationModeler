using AVAIntegrationModeler.UseCases.Contributors.Get;
using AVAIntegrationModeler.UseCases.Scenarios.Get;

namespace AVAIntegrationModeler.API.Scenarios;

/// <summary>
/// Get a Contributor by integer ID.
/// </summary>
/// <remarks>
/// Takes a positive integer ID and returns a matching Contributor record.
/// </remarks>
public class GetById(IMediator _mediator)
  : Endpoint<GetScenarioByIdRequest, ScenarioRecord>
{
  public override void Configure()
  {
    Get(GetScenarioByIdRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(GetScenarioByIdRequest request,
    CancellationToken cancellationToken)
  {
    var query = new GetScenarioQuery(request.ScenarioId);

    var result = await _mediator.Send(query, cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    if (result.IsSuccess)
    {
      
     Response = new ScenarioRecord(result.Value);
    }
  }
}
