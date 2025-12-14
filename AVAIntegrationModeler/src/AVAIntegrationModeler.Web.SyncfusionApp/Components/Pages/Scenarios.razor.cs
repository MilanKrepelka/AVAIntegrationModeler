using AVAIntegrationModeler.API.Client;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.JSInterop;
using static System.Net.WebRequestMethods;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Pages;

public partial class Scenarios : Microsoft.AspNetCore.Components.ComponentBase, IPageListBase
{
  [Inject]
  IAVAIntegrationModelerApiClient _apiClient { get; set; } = default!;

  
  [Inject] private NavigationManager NavigationManager { get; set; } = default!;


  /// <inheritdoc/>
  public bool IsLoading { get; set; } = false;
  /// <inheritdoc/>
  public Datasource Datasource { get; set; } = Datasource.Database;
  /// <inheritdoc/>
  public string FilterString { get; set; } = string.Empty;

  public List<ScenarioListViewModel> ScenariosList { get; set; } = new();

  protected async Task LoadItemsAsync()
  {
    try
    {
      IsLoading = true;
      //StateHasChanged(); // ✅ Aktualizace UI - zobrazení loading
      
      ScenariosList.Clear();
      
      // Načtení scénářů z AVAIntegrationModeler.API
      var scenarioListResponse = await _apiClient.GetScenarios(Datasource.AVAPlace, CancellationToken.None);

      if (scenarioListResponse?.Scenarios != null)
      {
        foreach (var scenario in scenarioListResponse.Scenarios)
        {
          ScenarioListViewModel? scenarioListViewModel = Mapping.ScenarioMapper.MapToScenarioListViewModel(scenario);
          if (scenarioListViewModel != null) 
          {
            ScenariosList.Add(scenarioListViewModel);
          }
        }
      }
    }
    catch (HttpRequestException httpEx)
    {
      Console.WriteLine($"Chyba HTTP požadavku: {httpEx.Message}");
      ScenariosList = new();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Chyba při načítání scénářů: {ex.Message}");
      ScenariosList = new();
    }
    finally
    {
      IsLoading = false;
      //StateHasChanged(); // ✅ Aktualizace UI - konec loading
    }
  }

  protected override async Task OnInitializedAsync()
  {
    await base.OnInitializedAsync();
    await LoadItemsAsync();
    if (Grid != null)
    {
      await Grid.Refresh(true);
    }
  }
}
