using AVAIntegrationModeler.UseCases.Contributors.Get;
using AVAIntegrationModeler.UseCases.Contributors.Update;
using AVAIntegrationModeler.UseCases.Scenarios.Update;

namespace AVAIntegrationModeler.API.Scenarios;

/// <summary>
/// Update an existing Scenario.
/// </summary>
/// <remarks>
/// Update an existing Scenario by providing a fully defined replacement set of values.
/// See: https://stackoverflow.com/questions/60761955/rest-update-best-practice-put-collection-id-without-id-in-body-vs-put-collecti
/// </remarks>
public class UpdateScenarioEndpoint(IMediator _mediator)
  : Endpoint<UpdateScenarioRequest, UpdateScenarioResponse>
{
  public override void Configure()
  {
    Put(UpdateScenarioRequest.Route);
    AllowAnonymous();
  }

  public override async Task HandleAsync(
    UpdateScenarioRequest request,
    CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(
      new UpdateScenarioCommand(request.Datasource, request.Scenario),
      cancellationToken);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(cancellationToken);
      return;
    }

    await SendOkAsync(new UpdateScenarioResponse(result.Value), cancellationToken);
    return;
    
  }
}
