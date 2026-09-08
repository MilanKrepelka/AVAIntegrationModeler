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

      var areasTask = ApiClient.GetAreas(Datasource.Database, CancellationToken.None);
      var dataModelsTask = ApiClient.GetDataModels(Datasource.Database, CancellationToken.None);
      await Task.WhenAll(areasTask, dataModelsTask);

      var areasResponse = areasTask.Result;
      var dataModelsResponse = dataModelsTask.Result;

      var allDataModels = dataModelsResponse?.DataModels ?? [];
      var dataModelsByArea = allDataModels
        .Where(dm => dm.AreaId.HasValue)
        .GroupBy(dm => dm.AreaId!.Value)
        .ToDictionary(g => g.Key, g => g.ToList());

      var newList = new List<AreaListViewModel>();
      if (areasResponse?.Areas != null)
      {
        foreach (var area in areasResponse.Areas)
        {
          var vm = Mapping.AreaMapper.MapToAreaListViewModel(area);
          if (vm == null) continue;

          if (dataModelsByArea.TryGetValue(area.Id, out var areaModels))
          {
            vm.DataModels = areaModels
              .Select(dm => Mapping.DataModelMapper.MapToViewModel(dm, allDataModels))
              .ToList();
          }

          newList.Add(vm);
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
