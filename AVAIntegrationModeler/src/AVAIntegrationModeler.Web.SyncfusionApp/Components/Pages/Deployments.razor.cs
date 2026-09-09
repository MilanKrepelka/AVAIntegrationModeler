using AVAIntegrationModeler.API.Client;
using AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Pages;

/// <summary>
/// Stránka pro zobrazení seznamu nasazení.
/// </summary>
public partial class Deployments : ComponentBase
{
  [Inject] IAVAIntegrationModelerApiClient ApiClient { get; set; } = default!;
  [Inject] private NavigationManager NavigationManager { get; set; } = default!;

  public bool IsLoading { get; set; } = false;
  public List<DeploymentListViewModel> DeploymentsList { get; set; } = new();

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
      await InvokeAsync(StateHasChanged);
      if (Grid != null) await Grid.Refresh();
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
      await ShowNotification($"Chyba exportu: {ex.Message}", false);
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
