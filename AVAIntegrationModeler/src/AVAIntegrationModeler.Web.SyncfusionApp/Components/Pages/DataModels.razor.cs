using AVAIntegrationModeler.API.Client;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using System.Text.Json.Serialization;

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
      DataModelList.Clear();


      var dataModelListResponse = await _apiClient.GetDataModels(Datasource.AVAPlace, CancellationToken.None);

      foreach (var dataModel in dataModelListResponse.DataModels)
      {
        DataModelListViewModel? dataModelListViewModel = Mapping.DataModelMapper.MapToViewModel(dataModel, dataModelListResponse.DataModels);
        if (dataModelListViewModel != null)
        {
          DataModelList.Add(dataModelListViewModel);
        }
      }

    }
    finally
    {
      IsLoading = false;
    }

  }

      

    private bool _initialized;

    protected override async Task OnInitializedAsync()
    {
        if (_initialized) return;
        _initialized = true;

        await base.OnInitializedAsync();
        await LoadItemsAsync();
        await Grid!.Refresh(true);
  }
}
