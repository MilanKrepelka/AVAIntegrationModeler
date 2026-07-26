using AVAIntegrationModeler.API.Client;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Pages;

public partial class DataModels : ComponentBase
{

  [Inject]
  IAVAIntegrationModelerApiClient _apiClient { get; set; } = default!;

  /// <inheritdoc/>
  public bool IsLoading { get; set; } = false;
    
    /// <inheritdoc/>
    public Datasource Datasource { get; set; } = Datasource.Database;
    
    /// <inheritdoc/>
    public string FilterString { get; set; } = string.Empty;

    public List<DataModelListViewModel> DataModelList { get; set; } = new();

  protected async Task LoadItemsAsync()
  {
    try
    {
      IsLoading = true;
      StateHasChanged();
      DataModelList.Clear();

      var dataModelListResponse = await _apiClient.GetDataModels(this.Datasource, CancellationToken.None);

      foreach (var dataModel in dataModelListResponse.DataModels)
      {
        DataModelListViewModel? dataModelListViewModel = Mapping.DataModelMapper.MapToViewModel(dataModel, dataModelListResponse.DataModels);
        if (dataModelListViewModel != null)
          DataModelList.Add(dataModelListViewModel);
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Chyba při načítání datových modelů: {ex.Message}");
      DataModelList = [];
    }
    finally
    {
      IsLoading = false;
      StateHasChanged();
    }
  }

  protected override async Task OnInitializedAsync()
  {
    await base.OnInitializedAsync();
    await LoadItemsAsync();
    if (Grid != null)
      await Grid.Refresh(true);
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
}
