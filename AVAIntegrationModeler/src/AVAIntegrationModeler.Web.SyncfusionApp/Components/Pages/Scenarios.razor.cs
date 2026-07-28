using AVAIntegrationModeler.API.Client;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.ScenarioAggregate;
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

  [Parameter] public string? Ds { get; set; }

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
      StateHasChanged(); // ✅ Aktualizace UI - zobrazení loading
      
      ScenariosList.Clear();

      // Načtení scénářů z AVAIntegrationModeler.API
      
      
      var scenarioListResponse = await _apiClient.GetScenarios(this.Datasource, CancellationToken.None);

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
      StateHasChanged(); // ✅ Aktualizace UI - konec loading
    }
  }

  private bool _initialized = false;

  protected override async Task OnParametersSetAsync()
  {
    var newDs = (Ds ?? "").Equals("avaplace", StringComparison.OrdinalIgnoreCase)
      ? Contracts.Datasource.AVAPlace
      : Contracts.Datasource.Database;

    if (_initialized && newDs == Datasource) return;

    _initialized = true;
    Datasource = newDs;
    await LoadItemsAsync();
    if (Grid != null) await Grid.Refresh(true);
  }

  protected override Task OnInitializedAsync() => base.OnInitializedAsync();
    private void AddNewScenario(Syncfusion.Blazor.Navigations.ClickEventArgs args)
    {
    NavigationManager.NavigateTo($"/scenarioedit/{this.Datasource}");
    
    //throw new NotImplementedException();
    }
}
