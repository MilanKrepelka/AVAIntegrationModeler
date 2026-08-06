using AVAIntegrationModeler.API.Client;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

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
      await ShowNotification($"Model '{model.Code}' importován do lokální DB. ID: {result.Value.ToString()[..8]}...");
    else if (result.Status == Ardalis.Result.ResultStatus.NotFound)
      await ShowNotification($"Model '{model.Code}' nebyl nalezen v AVAPlace.", false);
    else
      await ShowNotification($"Chyba importu '{model.Code}': {string.Join(", ", result.Errors)}", false);
  }

  private async Task ImportAllDataModelsAsync(Syncfusion.Blazor.Navigations.ClickEventArgs args)
  {
    var confirmed = await JS.InvokeAsync<bool>("confirm",
      "Opravdu chcete importovat všechny datové modely z AVAPlace do lokální databáze? Operace může u velkého počtu modelů trvat delší dobu.");
    if (!confirmed) return;

    IsLoading = true;
    await InvokeAsync(StateHasChanged);

    try
    {
      var result = await _apiClient.ImportAllDataModelsFromAvaPlace(CancellationToken.None);
      if (result.IsSuccess)
      {
        var summary = result.Value;
        var message = $"Import dokončen: {summary.SuccessCount} úspěšně, {summary.FailedCount} s chybou.";
        if (summary.FailedCount > 0)
          message += " Chyby: " + string.Join("; ", summary.Results
            .Where(r => !r.Success)
            .Select(r => $"{r.Code}: {r.ErrorMessage}"));
        await ShowNotification(message, summary.FailedCount == 0);
      }
      else
      {
        await ShowNotification($"Hromadný import se nezdařil: {string.Join(", ", result.Errors)}", false);
      }
    }
    finally
    {
      IsLoading = false;
      await InvokeAsync(StateHasChanged);
    }
  }

  public void Dispose()
  {
    _cts.Cancel();
    _cts.Dispose();
  }
}
