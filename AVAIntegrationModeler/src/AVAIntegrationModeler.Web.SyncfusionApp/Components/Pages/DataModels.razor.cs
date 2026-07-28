using AVAIntegrationModeler.API.Client;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Pages;

public partial class DataModels : ComponentBase, IDisposable
{
  [Inject] IAVAIntegrationModelerApiClient _apiClient { get; set; } = default!;

  [Parameter] public string? Ds { get; set; }

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

  private bool _initialized = false;

  protected override async Task OnParametersSetAsync()
  {
    var newDs = (Ds ?? "").Equals("avaplace", StringComparison.OrdinalIgnoreCase)
      ? Contracts.Datasource.AVAPlace
      : Contracts.Datasource.Database;

    if (_initialized && newDs == Datasource) return;

    _initialized = true;
    Datasource = newDs;
    await LoadItemsAsync();
  }

  protected override Task OnInitializedAsync() => base.OnInitializedAsync();

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

  private async Task ImportDataModelAsync(DataModelListViewModel model)
  {
    var result = await _apiClient.ImportDataModelFromAvaPlace(model.Id, CancellationToken.None);
    if (result.IsSuccess)
    {
      await ShowToast($"Model '{model.Code}' importován do lokální DB. ID: {result.Value.ToString()[..8]}...");
      await LoadItemsAsync();
    }
    else if (result.Status == Ardalis.Result.ResultStatus.NotFound)
      await ShowToast($"Model '{model.Code}' nebyl nalezen v AVAPlace.");
    else
      await ShowToast($"Chyba importu '{model.Code}': {string.Join(", ", result.Errors)}");
  }

  public void Dispose()
  {
    _cts.Cancel();
    _cts.Dispose();
  }
}
