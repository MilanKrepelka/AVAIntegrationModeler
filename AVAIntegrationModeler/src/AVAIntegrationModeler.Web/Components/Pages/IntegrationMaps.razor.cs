using AVAIntegrationModeler.API.IntegrationMaps;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Web.ViewModels.List;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Net.WebRequestMethods;
using static MudBlazor.CategoryTypes;
using static MudBlazor.Colors;

namespace AVAIntegrationModeler.Web.Components.Pages;

public partial class IntegrationMaps : Microsoft.AspNetCore.Components.ComponentBase, IPageListBase
{
  /// <inheritdoc/>
  public bool IsLoading { get; set; } = false;
  /// <inheritdoc/>
  public Datasource Datasource { get; set; } = Datasource.Database;
  /// <inheritdoc/>
  public string FilterString { get; set; } = string.Empty;

  public List<IntegrationMapListViewModel> ScenariosList { get; set; } = new();

  protected async Task onDatasourceChanded(Datasource datasource)
  {
    this.Datasource = datasource;
    await LoadItemsAsync();
  }

  

  protected bool FilterFunc(IntegrationMapListViewModel scenario)
  {
    if (string.IsNullOrEmpty(FilterString)) return true;
    return true;
    //return scenario.Code.Contains(FilterString, StringComparison.OrdinalIgnoreCase)
    //  || scenario.Id.ToString().Contains(FilterString, StringComparison.OrdinalIgnoreCase)
      
    //  || scenario.Name.CzechValue.ToString().Contains(FilterString, StringComparison.OrdinalIgnoreCase)
    //  || scenario.Name.EnglishValue.ToString().Contains(FilterString, StringComparison.OrdinalIgnoreCase)

    //  || scenario.Description.CzechValue.ToString().Contains(FilterString, StringComparison.OrdinalIgnoreCase)
    //  || scenario.Description.EnglishValue.ToString().Contains(FilterString, StringComparison.OrdinalIgnoreCase)

    //  || scenario.InputFeature.Code.Contains(FilterString, StringComparison.OrdinalIgnoreCase)
    //  || scenario.OutputFeature.Code.Contains(FilterString, StringComparison.OrdinalIgnoreCase)

    //  ;
  }
  protected async Task LoadItemsAsync()
  {
    try
    {
      IsLoading = true;
      ScenariosList.Clear();
      // Načtení scénářů z AVAIntegrationModeler.API
      using var httpClient = new HttpClient();
      var response = await httpClient.GetAsync($"http://localhost:57679/IntegrationMaps?datasource={this.Datasource}");
      response.EnsureSuccessStatusCode();

      var scenarioListResponse = await response.Content.ReadFromJsonAsync<IntegrationMapListResponse>();

      if (scenarioListResponse?.IntegrationMaps != null)
      {
        foreach (var integrationMap in scenarioListResponse?.IntegrationMaps!)
        {

          IntegrationMapListViewModel? integrationMapListViewModel = new IntegrationMapListViewModel();
          integrationMapListViewModel = Mapping.IntegrationMapMapper.MapToIntegrationMapSummaryListViewModel(integrationMap);
          if (integrationMapListViewModel != null) ScenariosList.Add(integrationMapListViewModel);
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
      Console.WriteLine($"Chyba při načítání integračních map: {ex.Message}");
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
