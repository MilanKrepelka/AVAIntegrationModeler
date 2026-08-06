using AVAIntegrationModeler.API.Client;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Web.SyncfusionApp.Mapping;
using AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Pages;

public partial class DataModelRecords : ComponentBase
{
  [Inject] private IAVAIntegrationModelerApiClient _apiClient { get; set; } = default!;
  [Inject] private NavigationManager NavigationManager { get; set; } = default!;
  [Inject] private IJSRuntime JS { get; set; } = default!;

  [Parameter] public string? Ds { get; set; }

  private SfGrid<DataModelRecordListViewModel>? Grid;

  public bool IsLoading { get; set; } = true;
  public Datasource Datasource { get; set; } = Datasource.Database;
  public Guid? SelectedModelId { get; set; }

  public List<DataModelRecordListViewModel> RecordList { get; set; } = new();

  private List<DataModelDTO> _dataModelList = new();

  private bool _initialized = false;

  protected override async Task OnParametersSetAsync()
  {
    var newDs = (Ds ?? "").Equals("avaplace", StringComparison.OrdinalIgnoreCase)
      ? Contracts.Datasource.AVAPlace
      : Contracts.Datasource.Database;

    if (_initialized && newDs == Datasource) return;

    _initialized = true;
    Datasource = newDs;
    SelectedModelId = null;
    await LoadDataModelsAsync();
    await LoadRecordsAsync();
    if (Grid != null) await Grid.Refresh();
  }

  protected override Task OnInitializedAsync() => base.OnInitializedAsync();

  private async Task LoadDataModelsAsync()
  {
    var response = await _apiClient.GetDataModels(Datasource, CancellationToken.None);
    _dataModelList = response.DataModels ?? new List<DataModelDTO>();
  }

  private async Task LoadRecordsAsync()
  {
    if (Datasource == Datasource.AVAPlace && !SelectedModelId.HasValue)
    {
      RecordList = new List<DataModelRecordListViewModel>();
      IsLoading = false;
      return;
    }

    IsLoading = true;
    var response = await _apiClient.GetDataModelRecords(Datasource, SelectedModelId, CancellationToken.None);
    RecordList = response.Records.Select(r => DataModelRecordMapper.MapToViewModel(r, _dataModelList)).ToList();
    IsLoading = false;
  }

  private async Task HandleModelChange(Guid? modelId)
  {
    SelectedModelId = modelId;
    await LoadRecordsAsync();
    if (Grid != null) await Grid.Refresh();
  }

  private void AddNewRecord()
  {
    if (SelectedModelId.HasValue)
      NavigationManager.NavigateTo($"/datamodelrecordedit/{Datasource}/{SelectedModelId.Value}");
  }

  private void EditRecord(DataModelRecordListViewModel record)
    => NavigationManager.NavigateTo($"/datamodelrecordedit/{Datasource}/{record.ModelId}/{record.Id}");

  private async Task DeleteRecordAsync(DataModelRecordListViewModel record)
  {
    var confirmed = await JS.InvokeAsync<bool>("confirm", $"Opravdu smazat záznam '{record.ExternalId}'?");
    if (!confirmed) return;

    await _apiClient.DeleteDataModelRecord(Datasource, record.Id, CancellationToken.None);
    await LoadRecordsAsync();
    if (Grid != null) await Grid.Refresh();
  }

  private async Task ExportAsync()
  {
    if (Grid is null) return;
    var selected = await Grid.GetSelectedRecordsAsync();
    if (!selected.Any()) return;
    var ids = selected.Select(r => r.Id).ToList();
    var bytes = await _apiClient.ExportDataModelRecords(Datasource, ids, CancellationToken.None);
    await JS.InvokeVoidAsync("downloadFile", "export.zip", "application/zip", bytes);
  }
}
