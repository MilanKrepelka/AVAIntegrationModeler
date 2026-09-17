using AVAIntegrationModeler.API.Client;
using AVAIntegrationModeler.Localization;
using AVAIntegrationModeler.Web.SyncfusionApp.Services;
using AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Pages;

/// <summary>
/// Stránka pro zobrazení seznamu nasazení.
/// </summary>
public partial class Deployments : ComponentBase
{
  [Inject] IAVAIntegrationModelerApiClient ApiClient { get; set; } = default!;
  [Inject] private NavigationManager NavigationManager { get; set; } = default!;
  [Inject] GridFilterStateService _filterState { get; set; } = default!;

  public bool IsLoading { get; set; } = false;
  public List<DeploymentListViewModel> DeploymentsList { get; set; } = new();

  private List<GridFilterColumn> _filterPredicates = new();
  private bool _gridVisible = true;
  private bool _pendingFilterRestore = false;

  protected override void OnInitialized()
  {
    _filterPredicates = _filterState.GetOrEmpty("Deployments");
    _pendingFilterRestore = _filterPredicates.Count > 0;
  }

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

      var newList = new List<DeploymentListViewModel>();
      var response = await ApiClient.GetDeployments(CancellationToken.None);
      if (response?.Deployments != null)
      {
        foreach (var dep in response.Deployments)
        {
          var vm = Mapping.DeploymentMapper.MapToDeploymentListViewModel(dep);
          if (vm != null) newList.Add(vm);
        }
      }

      DeploymentsList = newList;
    }
    catch (HttpRequestException ex)
    {
      Console.WriteLine($"Chyba HTTP požadavku: {ex.Message}");
      DeploymentsList = new();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Chyba při načítání nasazení: {ex.Message}");
      DeploymentsList = new();
    }
    finally
    {
      IsLoading = false;
      if (_pendingFilterRestore) { IsLoading = true; _gridVisible = false; }
      await InvokeAsync(StateHasChanged);
      if (Grid != null) await Grid.Refresh();
      await RestoreFiltersAsync();
    }
  }

  private async Task RestoreFiltersAsync()
  {
    if (!_pendingFilterRestore) return;
    _pendingFilterRestore = false;
    try
    {
      if (Grid != null)
        foreach (var col in _filterPredicates)
          if (col.Value != null)
            await Grid.FilterByColumnAsync(col.Field, col.Operator.ToString().ToLower(), col.Value, col.Predicate ?? "and", col.MatchCase);
    }
    finally
    {
      _gridVisible = true;
      IsLoading = false;
      await InvokeAsync(StateHasChanged);
    }
  }

  private void AddNew(Syncfusion.Blazor.Navigations.ClickEventArgs args)
  {
    NavigationManager.NavigateTo("/deploymentedit");
  }

  private async Task ExportDeploymentAsync(DeploymentListViewModel deployment)
  {
    try
    {
      var bytes = await ApiClient.ExportDeployment(deployment.Code, CancellationToken.None);
      var fileName = $"deployment-{deployment.Code}-export.zip";
      await JS.InvokeVoidAsync("downloadFile", fileName, "application/zip", bytes);
    }
    catch (Exception ex)
    {
      await ShowNotification($"Export error: {ex.Message}", false);
    }
  }

  private async Task DeleteDeploymentAsync(DeploymentListViewModel deployment)
  {
    var confirmed = await JS.InvokeAsync<bool>("confirm", $"Opravdu smazat nasazení '{deployment.Code}'?");
    if (!confirmed) return;

    var result = await ApiClient.DeleteDeployment(deployment.Code, CancellationToken.None);
    await ShowNotification(
      result.IsSuccess
        ? $"Nasazení '{deployment.Code}' bylo smazáno."
        : $"Nasazení se nepodařilo smazat: {string.Join(", ", result.Errors)}",
      result.IsSuccess);

    if (result.IsSuccess)
      await LoadItemsAsync();
  }
}
