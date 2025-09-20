using AVAIntegrationModeler.API.Scenarios.ViewModels;
using AVAIntegrationModeler.UseCases.Scenarios;
using AVAIntegrationModeler.UseCases.Scenarios.List;

namespace AVAIntegrationModeler.API.Scenarios;

/// <summary>
/// List all Scenarios
/// </summary>
/// <remarks>
/// List all contributors - returns a ContributorListResponse containing the Scenarios.
/// </remarks>
public class ListFullPreview(IMediator _mediator) : EndpointWithoutRequest<ScenarioListFullPreviewResponse>
{
  public override void Configure()
  {
    Get("/Scenarios/FullPreview");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new ListScenariosQuery(null, null), cancellationToken);

    var result2 = await new ListScenariosQuery2(null, null)
      .ExecuteAsync(cancellationToken);

    if (result.IsSuccess)
    {
      Response = new ScenarioListFullPreviewResponse
      {
        Scenarios = result.Value.Select(scenatioDTO => 
        new ScenarioFullPreview(scenatioDTO) 
        {  
          InputFeatureCode = "Potřeba načíst feature",
          OutputFeatureCode = "Potřeba načíst feature"
        }
        ).ToList()
      };
    }
  }
}
