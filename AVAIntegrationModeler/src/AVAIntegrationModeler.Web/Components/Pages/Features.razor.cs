using AVAIntegrationModeler.API.Features;
using AVAIntegrationModeler.API.Scenarios;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Web.ViewModels.List;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Net.WebRequestMethods;
using static MudBlazor.CategoryTypes;
using static MudBlazor.Colors;

namespace AVAIntegrationModeler.Web.Components.Pages;

public partial class Features : Microsoft.AspNetCore.Components.ComponentBase, IPageListBase
{
  /// <inheritdoc/>
  public bool IsLoading { get; set; } = false;
  /// <inheritdoc/>
  public Datasource Datasource { get; set; } = Datasource.Database;
  /// <inheritdoc/>
  public string FilterString { get; set; } = string.Empty;

  public List<FeatureListViewModel> FeatureList { get; set; } = new();

  protected async Task onDatasourceChanded(Datasource datasource)
  {
    this.Datasource = datasource;
    await LoadItemsAsync();
  }


  /// <summary>
  /// Vyhledávací funkce pro filtrování funkcí na základě filtračního řetězce.
  /// </summary>
  /// <param name="feature">Feature</param>
  /// <returns>Příznak, že <paramref name="feature"/> patří do filtru</returns>
  protected bool FilterFunc(FeatureListViewModel feature)
  {
    if (string.IsNullOrEmpty(FilterString)) return true;

    return feature.Code.Contains(FilterString, StringComparison.OrdinalIgnoreCase)
      || feature.Id.ToString().Contains(FilterString, StringComparison.OrdinalIgnoreCase)

      || feature.Name.CzechValue.ToString().Contains(FilterString, StringComparison.OrdinalIgnoreCase)
      || feature.Name.EnglishValue.ToString().Contains(FilterString, StringComparison.OrdinalIgnoreCase)

      || feature.Description.CzechValue.ToString().Contains(FilterString, StringComparison.OrdinalIgnoreCase)
      || feature.Description.EnglishValue.ToString().Contains(FilterString, StringComparison.OrdinalIgnoreCase);
  }
  protected async Task LoadItemsAsync()
  {
    try
    {
      IsLoading = true;
      FeatureList.Clear();
      // Načtení scénářů z AVAIntegrationModeler.API
      using var httpClient = new HttpClient();
      var response = await httpClient.GetAsync($"http://localhost:57679/DataModels?datasource={this.Datasource}");
      response.EnsureSuccessStatusCode();

      var scenarioListResponse = await response.Content.ReadFromJsonAsync<FeatureListResponse>();

      if (scenarioListResponse?.Features!= null)
      {
        foreach (var feature in scenarioListResponse?.Features!)
        {

          FeatureListViewModel? featureListViewModel = new FeatureListViewModel();
          featureListViewModel = Mapping.FeatureMapper.MapToViewModel(feature);
          if (featureListViewModel != null) FeatureList.Add(featureListViewModel);
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
      FeatureList = new();
    }
    finally
    {
      IsLoading = false;
    }
  }
  protected void ShowBtnPress(ScenarioListViewModel scenarioListViewModel)
  {
    scenarioListViewModel.ShowDetails = !scenarioListViewModel.ShowDetails;
  }


  protected override async Task OnInitializedAsync()
  {
    base.OnInitialized();
    await LoadItemsAsync();
  }
}
