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
public class ListFullPreview(IMediator _mediator) : Endpoint<ScenarioListFullPreviewRequest,ScenarioListFullPreviewResponse>
{
  public override void Configure()
  {
    Get("/Scenarios/FullPreview");
    AllowAnonymous();
  }

  public override async Task HandleAsync(ScenarioListFullPreviewRequest request, CancellationToken cancellationToken)
  {
    var result = await _mediator.Send(new ListScenariosQuery( request.Datasource, null, null), cancellationToken);

    var result2 = await new ListScenariosQuery2(request.Datasource,  null, null)
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
