using AVAIntegrationModeler.API.Client;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Localization;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Web.SyncfusionApp.Mapping;
using AVAIntegrationModeler.Web.SyncfusionApp.Services;
using AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Pages;

public partial class DataModelRecords : ComponentBase
{
  [Inject] private IAVAIntegrationModelerApiClient _apiClient { get; set; } = default!;
  [Inject] private NavigationManager NavigationManager { get; set; } = default!;
  [Inject] private IJSRuntime JS { get; set; } = default!;
  [Inject] private GridFilterStateService _filterState { get; set; } = default!;

  [Parameter] public string? Ds { get; set; }

  private SfGrid<DataModelRecordListViewModel>? Grid;
  private static readonly FilterSettings _containsFilter = new() { Operator = Syncfusion.Blazor.Operator.Contains };

  public bool IsLoading { get; set; } = true;
  public Datasource Datasource { get; set; } = Datasource.Database;
  public Guid? SelectedModelId { get; set; }

  private List<GridFilterColumn> _filterPredicates = new();
  private bool _gridVisible = true;
  private bool _pendingFilterRestore = false;

  public List<DataModelRecordListViewModel> RecordList { get; set; } = new();

  private List<DataModelDTO> _dataModelList = new();

  private bool _initialized = false;
  private bool _importLoading = false;
  private ImportDataModelRecordsXlsResult? _importResult;

  protected override async Task OnParametersSetAsync()
  {
    var newDs = (Ds ?? "").Equals("avaplace", StringComparison.OrdinalIgnoreCase)
      ? Contracts.Datasource.AVAPlace
      : Contracts.Datasource.Database;

    if (_initialized && newDs == Datasource) return;

    _initialized = true;
    Datasource = newDs;
    _filterPredicates = _filterState.GetOrEmpty($"DataModelRecords_{newDs}");
    _pendingFilterRestore = _filterPredicates.Count > 0;
    SelectedModelId = null;
    await LoadDataModelsAsync();
    await LoadRecordsAsync();
    if (_pendingFilterRestore) _gridVisible = false;
    await InvokeAsync(StateHasChanged);
    if (Grid != null) await Grid.Refresh();
    await RestoreFiltersAsync();
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

  private void ViewRecord(DataModelRecordListViewModel record)
    => NavigationManager.NavigateTo($"/datamodelrecordview/{Datasource}/{record.ModelId}/{record.Id}?returnUrl=/datamodelrecords/{Ds}");

  private async Task DeleteRecordAsync(DataModelRecordListViewModel record)
  {
    var confirmed = await JS.InvokeAsync<bool>("confirm", string.Format(SharedResources.DataModelRecords_DeleteConfirm, record.ExternalId));
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

  private async Task ExportToXlsAsync()
  {
    if (!SelectedModelId.HasValue) return;
    var (content, fileName) = await _apiClient.ExportDataModelRecordsToXls(
      Datasource, SelectedModelId.Value, CancellationToken.None);
    await JS.InvokeVoidAsync("downloadFile", fileName,
      "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", content);
  }

  private async Task OpenXlsFilePicker()
  {
    await JS.InvokeVoidAsync("triggerClick", "xls-import-input");
  }

  private async Task ImportFromXlsAsync(InputFileChangeEventArgs e)
  {
    if (!SelectedModelId.HasValue || e.File is null) return;

    _importLoading = true;
    _importResult = null;
    await InvokeAsync(StateHasChanged);

    try
    {
      using var ms = new MemoryStream();
      await e.File.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024).CopyToAsync(ms);
      var bytes = ms.ToArray();

      var result = await _apiClient.ImportDataModelRecordsFromXls(
        Datasource, SelectedModelId.Value, bytes, e.File.Name, CancellationToken.None);

      if (result.IsSuccess)
      {
        _importResult = result.Value;
        if (result.Value.CreatedCount > 0 || result.Value.UpdatedCount > 0)
        {
          await LoadRecordsAsync();
          if (Grid != null) await Grid.Refresh();
        }
      }
    }
    finally
    {
      _importLoading = false;
      await InvokeAsync(StateHasChanged);
    }
  }

  private void OnGridActionCompleted(ActionEventArgs<DataModelRecordListViewModel> args)
  {
    if (args.RequestType is Syncfusion.Blazor.Grids.Action.Filtering or Syncfusion.Blazor.Grids.Action.ClearFiltering)
      _filterState.Save($"DataModelRecords_{Datasource}", Grid?.FilterSettings?.Columns);
  }
}
