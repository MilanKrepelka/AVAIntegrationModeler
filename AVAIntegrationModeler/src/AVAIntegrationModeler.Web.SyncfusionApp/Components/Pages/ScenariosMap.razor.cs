using AVAIntegrationModeler.API.Client;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.JSInterop;
using static System.Net.WebRequestMethods;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Pages;

public partial class ScenariosMap : Microsoft.AspNetCore.Components.ComponentBase
{
  [Inject]
  IAVAIntegrationModelerApiClient _apiClient { get; set; } = default!;

  [Parameter] public string scenarioCode { get; set; } = string.Empty;

  ScenarioDTO _scenarioDTO = new ScenarioDTO();
  /// <inheritdoc/>
  public bool IsLoading { get; set; } = false;

  //protected override async Task OnInitializedAsync()
  //{
  //  await base.OnInitializedAsync();
  //  if (string.IsNullOrEmpty(scenarioCode))
  //  {
  //    return;
  //  }
  //  _scenarioDTO = await _apiClient.GetScenario(Datasource.AVAPlace, scenarioCode, CancellationToken.None);
  //  InitDiagramModel();
    
  //}
}
