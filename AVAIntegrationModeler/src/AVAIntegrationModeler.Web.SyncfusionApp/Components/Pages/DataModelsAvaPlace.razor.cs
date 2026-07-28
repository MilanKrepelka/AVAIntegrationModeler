using AVAIntegrationModeler.API.Client;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;
using Microsoft.AspNetCore.Components;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Pages;

public partial class DataModelsAvaPlace : ComponentBase, IDisposable
{
  [Inject] IAVAIntegrationModelerApiClient _apiClient { get; set; } = default!;

  public bool IsLoading { get; set; } = false;
  public Datasource Datasource { get; set; } = Datasource.AVAPlace;
  public List<DataModelListViewModel> DataModelList { get; set; } = new();

  private CancellationTokenSource _cts = new();

  protected async Task LoadItemsAsync()
  {
    _cts.Cancel();
    _cts.Dispose();
    _cts = new CancellationTokenSource();
    var token = _cts.Token;

    try
    {
      IsLoading = true;
      await InvokeAsync(StateHasChanged);

      var newList = new List<DataModelListViewModel>();
      var response = await _apiClient.GetDataModels(Datasource.AVAPlace, token);

      foreach (var dataModel in response.DataModels)
      {
        var vm = Mapping.DataModelMapper.MapToViewModel(dataModel, response.DataModels);
        if (vm != null) newList.Add(vm);
      }

      DataModelList = newList;
    }
    catch (OperationCanceledException) { }
    catch (Exception ex)
    {
      Console.WriteLine($"Chyba při načítání datových modelů (AVAPlace): {ex.Message}");
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

  private async Task ImportDataModelAsync(DataModelListViewModel model)
  {
    var result = await _apiClient.ImportDataModelFromAvaPlace(model.Id, CancellationToken.None);
    if (result.IsSuccess)
      await ShowToast($"Model '{model.Code}' importován do lokální DB. ID: {result.Value.ToString()[..8]}...");
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
