using AVAIntegrationModeler.API.Client;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Pages;

public partial class DataModels : ComponentBase, IDisposable
{
  [Inject] IAVAIntegrationModelerApiClient _apiClient { get; set; } = default!;

  public bool IsLoading { get; set; } = false;
  public Datasource Datasource { get; set; } = Datasource.Database;
  public string FilterString { get; set; } = string.Empty;
  public List<DataModelListViewModel> DataModelList { get; set; } = new();

  private CancellationTokenSource _cts = new();

  protected async Task LoadItemsAsync()
  {
    // Cancel any in-progress load (previous call or prior datasource switch)
    _cts.Cancel();
    _cts.Dispose();
    _cts = new CancellationTokenSource();
    var token = _cts.Token;

    try
    {
      IsLoading = true;
      await InvokeAsync(StateHasChanged);

      var newList = new List<DataModelListViewModel>();
      var dataModelListResponse = await _apiClient.GetDataModels(this.Datasource, token);

      foreach (var dataModel in dataModelListResponse.DataModels)
      {
        var vm = Mapping.DataModelMapper.MapToViewModel(dataModel, dataModelListResponse.DataModels);
        if (vm != null) newList.Add(vm);
      }

      DataModelList = newList;
    }
    catch (OperationCanceledException) { }
    catch (Exception ex)
    {
      Console.WriteLine($"Chyba při načítání datových modelů: {ex.Message}");
      DataModelList = [];
    }
    finally
    {
      if (!token.IsCancellationRequested)
      {
        IsLoading = false;
        await InvokeAsync(StateHasChanged);
      }
    }
  }

  protected override async Task OnInitializedAsync()
  {
    await base.OnInitializedAsync();
    await LoadItemsAsync();
  }

  private void AddNewDataModel()
    => NavigationManager.NavigateTo($"/datamodeledit/{Datasource}");

  private async Task DeleteDataModelAsync(DataModelListViewModel model)
  {
    var confirmed = await JS.InvokeAsync<bool>("confirm", $"Opravdu chcete smazat datový model '{model.Code}'?");
    if (!confirmed) return;

    var result = await _apiClient.DeleteDataModel(Datasource.Database, model.Id, CancellationToken.None);
    if (result.IsSuccess)
    {
      DataModelList.Remove(model);
      if (Grid != null) await Grid.Refresh();
    }
    else
    {
      await ShowToast($"Chyba při mazání: {string.Join(", ", result.Errors)}");
    }
  }

  private async Task ExportAsync()
  {
    if (Grid is null) return;
    var selected = await Grid.GetSelectedRecordsAsync();
    if (!selected.Any()) return;
    var ids = selected.Select(r => r.Id).ToList();
    var bytes = await _apiClient.ExportDataModels(Datasource, ids, CancellationToken.None);
    await JS.InvokeVoidAsync("downloadFile", "export.zip", "application/zip", bytes);
  }

  public void Dispose()
  {
    _cts.Cancel();
    _cts.Dispose();
  }
}
