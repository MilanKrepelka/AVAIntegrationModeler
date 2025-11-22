using AVAIntegrationModeler.API.Scenarios;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Net.WebRequestMethods;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Pages;

public partial class Scenarios : Microsoft.AspNetCore.Components.ComponentBase, IPageListBase
{
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
      using var httpClient = new HttpClient();
      var response = await httpClient.GetAsync($"http://localhost:57679/Scenarios?datasource={this.Datasource}");
      response.EnsureSuccessStatusCode();

      var scenarioListResponse = await response.Content.ReadFromJsonAsync<ScenarioListResponse>();
      
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
  }
}
