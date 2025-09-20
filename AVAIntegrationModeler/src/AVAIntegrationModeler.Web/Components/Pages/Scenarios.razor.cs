using AVAIntegrationModeler.API.Scenarios;
using AVAIntegrationModeler.API.Scenarios.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Net.WebRequestMethods;
using static MudBlazor.CategoryTypes;

namespace AVAIntegrationModeler.Web.Components.Pages;

public partial class Scenarios : Microsoft.AspNetCore.Components.ComponentBase
{
  protected bool IsLoading { get; set; } = false;

  protected List<ScenarioFullPreview> ScenariosList { get; set; } = new();


  protected async Task LoadItemsAsync()
  {
    try
    {
      IsLoading = true;

      // Načtení scénářů z AVAIntegrationModeler.API
      using var httpClient = new HttpClient();
      var response = await httpClient.GetAsync("http://localhost:57679/scenarios/FullPreview");
      response.EnsureSuccessStatusCode();

      var scenarioListResponse = await response.Content.ReadFromJsonAsync<ScenarioListFullPreviewResponse>();
      ScenariosList = scenarioListResponse?.Scenarios!;
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

  protected override async Task OnInitializedAsync()
  {
    base.OnInitialized();
    await LoadItemsAsync();
  }
}
