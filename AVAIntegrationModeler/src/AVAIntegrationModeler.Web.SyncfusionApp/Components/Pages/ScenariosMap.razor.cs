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

  [Parameter] public string dataSourceAsString { get; set; } = string.Empty;

  ScenarioDTO _scenarioDTO = new ScenarioDTO();
  /// <inheritdoc/>
  public bool IsLoading { get; set; } = false;

  protected override async Task OnInitializedAsync()
  {
    await base.OnInitializedAsync();
    if (string.IsNullOrEmpty(scenarioCode))
    {
      return;
    }
    if (!Enum.TryParse<Datasource>(dataSourceAsString, true, out var datasource))
    {
      // Fallback na Database pokud parsing selže
      datasource = Datasource.Database;
    }

    _scenarioDTO = await _apiClient.GetScenario(datasource, scenarioCode, CancellationToken.None);

    // Build model
    InitDiagramModel();

    // Defer layout until @ref is ready
    _needsLayout = true;

    

  }
}
