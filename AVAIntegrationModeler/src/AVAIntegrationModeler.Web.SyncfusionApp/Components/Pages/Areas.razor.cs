using AVAIntegrationModeler.API.Client;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Pages;

/// <summary>
/// Stránka pro zobrazení seznamu oblastí.
/// </summary>
public partial class Areas : ComponentBase
{
  [Inject] IAVAIntegrationModelerApiClient ApiClient { get; set; } = default!;
  [Inject] private NavigationManager NavigationManager { get; set; } = default!;

  public bool IsLoading { get; set; } = false;
  public List<AreaListViewModel> AreasList { get; set; } = new();

  protected override async Task OnInitializedAsync()
  {
    await LoadItemsAsync();
  }

  private async Task LoadItemsAsync()
  {
    try
    {
      IsLoading = true;
      await InvokeAsync(StateHasChanged);

      var newList = new List<AreaListViewModel>();
      var response = await ApiClient.GetAreas(Datasource.Database, CancellationToken.None);
      if (response?.Areas != null)
      {
        foreach (var area in response.Areas)
        {
          var vm = Mapping.AreaMapper.MapToAreaListViewModel(area);
          if (vm != null) newList.Add(vm);
        }
      }

      AreasList = newList;
    }
    catch (HttpRequestException ex)
    {
      Console.WriteLine($"Chyba HTTP požadavku: {ex.Message}");
      AreasList = new();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Chyba při načítání oblastí: {ex.Message}");
      AreasList = new();
    }
    finally
    {
      IsLoading = false;
      await InvokeAsync(StateHasChanged);
      if (Grid != null) await Grid.Refresh();
    }
  }

  private void AddNew(Syncfusion.Blazor.Navigations.ClickEventArgs args)
  {
    NavigationManager.NavigateTo("/areaedit");
  }

  private async Task DeleteAreaAsync(AreaListViewModel area)
  {
    var confirmed = await JS.InvokeAsync<bool>("confirm", $"Opravdu smazat oblast '{area.Code}'?");
    if (!confirmed) return;

    var result = await ApiClient.DeleteArea(Datasource.Database, area.Code, CancellationToken.None);
    await ShowNotification(
      result.IsSuccess
        ? $"Oblast '{area.Code}' byla smazána."
        : $"Oblast se nepodařilo smazat: {string.Join(", ", result.Errors)}",
      result.IsSuccess);

    if (result.IsSuccess)
      await LoadItemsAsync();
  }
}
