using AVAIntegrationModeler.API.Client;
using AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.DataModelChanges;
using Microsoft.AspNetCore.Components;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Pages;

/// <summary>
/// Stránka zobrazující hluboké porovnání datového modelu mezi databází a AVAPlace.
/// </summary>
public partial class DataModelChanges : ComponentBase, IDisposable
{
  [Parameter] public Guid Id { get; set; }

  [Inject] private IAVAIntegrationModelerApiClient ApiClient { get; set; } = default!;
  [Inject] private NavigationManager NavigationManager { get; set; } = default!;

  public bool IsLoading { get; private set; } = true;
  public string? ErrorMessage { get; private set; }
  public DataModelChangesViewModel ViewModel { get; private set; } = new();

  private bool _disposed;

  protected override async Task OnInitializedAsync() => await LoadAsync();

  private async Task LoadAsync()
  {
    IsLoading = true;
    ErrorMessage = null;
    await InvokeAsync(StateHasChanged);
    try
    {
      var comparison = await ApiClient.GetDataModelChanges(Id, CancellationToken.None);
      if (_disposed) return;
      if (comparison is null)
        ErrorMessage = $"DataModel s ID {Id} nebyl nalezen v žádném datovém zdroji.";
      else
        ViewModel = new DataModelChangesViewModel { Comparison = comparison };
    }
    catch (Exception ex)
    {
      if (!_disposed)
        ErrorMessage = $"Chyba při načítání: {ex.Message}";
    }
    finally
    {
      if (!_disposed)
      {
        IsLoading = false;
        await InvokeAsync(StateHasChanged);
      }
    }
  }

  private async Task ReloadAsync()
  {
    ViewModel = new DataModelChangesViewModel();
    await LoadAsync();
  }

  private void ToggleSameFields() => ViewModel.ShowSameFields = !ViewModel.ShowSameFields;

  private void GoBack() => NavigationManager.NavigateTo("/datamodels/database");

  public void Dispose() => _disposed = true;
}
