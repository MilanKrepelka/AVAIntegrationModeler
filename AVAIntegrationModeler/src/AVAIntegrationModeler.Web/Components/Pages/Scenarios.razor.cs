using AVAIntegrationModeler.API.Scenarios;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Web.ViewModels.List;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Net.WebRequestMethods;
using static MudBlazor.CategoryTypes;
using static MudBlazor.Colors;

namespace AVAIntegrationModeler.Web.Components.Pages;

public partial class Scenarios : Microsoft.AspNetCore.Components.ComponentBase
{
  protected bool IsLoading { get; set; } = false;

  protected Datasource Datasource = Datasource.Database;

  protected List<ScenarioListViewModel> ScenariosList { get; set; } = new();

  protected async Task onDatasourceChanded(Datasource datasource)
  {
    this.Datasource = datasource;
    await LoadItemsAsync();
  }

  protected string filterString = "";

  protected bool FilterFunc(ScenarioListViewModel scenario)
  {
    if (string.IsNullOrEmpty(filterString)) return true;

    return scenario.Code.Contains(filterString, StringComparison.OrdinalIgnoreCase)
      || scenario.Id.ToString().Contains(filterString, StringComparison.OrdinalIgnoreCase)
      
      || scenario.Name.CzechValue.ToString().Contains(filterString, StringComparison.OrdinalIgnoreCase)
      || scenario.Name.EnglishValue.ToString().Contains(filterString, StringComparison.OrdinalIgnoreCase)

      || scenario.Description.CzechValue.ToString().Contains(filterString, StringComparison.OrdinalIgnoreCase)
      || scenario.Description.EnglishValue.ToString().Contains(filterString, StringComparison.OrdinalIgnoreCase)

      || scenario.InputFeature.Code.Contains(filterString, StringComparison.OrdinalIgnoreCase)
      || scenario.OutputFeature.Code.Contains(filterString, StringComparison.OrdinalIgnoreCase)

      ;
  }
  protected async Task LoadItemsAsync()
  {
    try
    {
      IsLoading = true;
      ScenariosList.Clear();
      // Načtení scénářů z AVAIntegrationModeler.API
      using var httpClient = new HttpClient();
      var response = await httpClient.GetAsync($"http://localhost:57679/Scenarios?datasource={this.Datasource}");
      response.EnsureSuccessStatusCode();

      var scenarioListResponse = await response.Content.ReadFromJsonAsync<ScenarioListResponse>();
      
      if (scenarioListResponse?.Scenarios != null)
      {
        foreach (var scenario in scenarioListResponse?.Scenarios!)
        {

          ScenarioListViewModel? scenarioListViewModel = new ScenarioListViewModel();
          scenarioListViewModel = Mapping.ScenarioMapper.MapToScenarioListViewModel(scenario);
          if (scenarioListViewModel != null)ScenariosList.Add(scenarioListViewModel);
        }
      }
    }
    
    catch (HttpRequestException httpEx)
    {
      Console.WriteLine($"Chyba HTTP požadavku: {httpEx.Message}");
      // fallback na lokální data
      
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Chyba při načítání scénářů: {ex.Message}");
      // fallback na lokální data
      ScenariosList = new();
    }
    finally
    {
      IsLoading = false;
    }
  }
  protected void ShowBtnPress(ScenarioListViewModel scenarioListViewModel )
  {
    scenarioListViewModel.ShowDetails = !scenarioListViewModel.ShowDetails;
  }


  protected override async Task OnInitializedAsync()
  {
    base.OnInitialized();
    await LoadItemsAsync();
  }
}
