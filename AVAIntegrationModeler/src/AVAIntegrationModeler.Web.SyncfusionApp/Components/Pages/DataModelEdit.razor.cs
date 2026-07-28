using System.ComponentModel.DataAnnotations;
using AVAIntegrationModeler.API.Client;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Web.SyncfusionApp.Components.Widgets;
using Microsoft.AspNetCore.Components;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Pages;

public partial class DataModelEdit : ComponentBase, IDisposable
{
  private bool _disposed = false;
  private CancellationTokenSource? _cts;

  [Inject] private IAVAIntegrationModelerApiClient _apiClient { get; set; } = default!;
  [Inject] private NavigationManager NavigationManager { get; set; } = default!;

  [Parameter] public Guid? id { get; set; }
  [Parameter] public string dataSourceAsString { get; set; } = string.Empty;

  private Datasource _datasource = Datasource.Database;
  private bool IsNew => id is null || id == Guid.Empty;

  private List<DataModelFieldEditModel> _fields = [];
  private List<DataModelSummaryDTO> _availableModels = [];

  private class DataModelEditModel
  {
    public Guid Id { get; set; }
    public string IdText { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kód je povinný.")]
    [StringLength(200)]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Název je povinný.")]
    [StringLength(500)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [StringLength(2000)]
    public string Notes { get; set; } = string.Empty;

    public bool IsAggregateRoot { get; set; } = false;
  }

  private DataModelEditModel _edit = new();

  protected override void OnInitialized() => _cts = new CancellationTokenSource();

  protected override async Task OnParametersSetAsync()
  {
    if (_disposed || _cts?.Token.IsCancellationRequested == true) return;

    if (!Enum.TryParse<Datasource>(dataSourceAsString, true, out var ds))
      ds = Datasource.Database;
    _datasource = ds;

    var token = _cts?.Token ?? CancellationToken.None;

    if (IsNew)
    {
      var newId = Guid.NewGuid();
      _edit = new DataModelEditModel { Id = newId, IdText = newId.ToString() };
      _fields = [];
    }
    else
    {
      try
      {
        var dto = await _apiClient.GetDataModel(_datasource, id!.Value, token);
        _edit = new DataModelEditModel
        {
          Id = dto.Id,
          IdText = dto.Id.ToString(),
          Code = dto.Code,
          Name = dto.Name,
          Description = dto.Description,
          Notes = dto.Notes,
          IsAggregateRoot = dto.IsAggregateRoot
        };
        _fields = dto.Fields.Select(f => new DataModelFieldEditModel
        {
          Id = f.Id,
          Name = f.Name,
          Label = f.Label,
          Description = f.Description,
          FieldType = f.FieldType,
          IsPublishedForLookup = f.IsPublishedForLookup,
          IsCollection = f.IsCollection,
          IsLocalized = f.IsLocalized,
          IsNullable = f.IsNullable,
          ReferencedEntityTypeIds = [.. f.ReferencedEntityTypeIds]
        }).ToList();
      }
      catch (OperationCanceledException) { }
      catch (Exception ex) { Console.WriteLine($"Error loading DataModel: {ex.Message}"); }
    }

    try
    {
      var modelsResponse = await _apiClient.GetDataModels(Datasource.Database, token);
      _availableModels = modelsResponse.DataModels
        .Select(m => new DataModelSummaryDTO { Id = m.Id, Code = m.Code, Name = m.Name })
        .OrderBy(m => m.Code)
        .ToList();
    }
    catch (Exception ex) { Console.WriteLine($"Error loading available models: {ex.Message}"); }
  }

  private async Task SaveAsync()
  {
    if (_disposed || _cts?.Token.IsCancellationRequested == true) return;

    var dto = new DataModelDTO
    {
      Id = _edit.Id,
      Code = _edit.Code,
      Name = _edit.Name,
      Description = _edit.Description,
      Notes = _edit.Notes,
      IsAggregateRoot = _edit.IsAggregateRoot,
      Fields = _fields.Select(f => new DataModelFieldDTO
      {
        Id = f.Id,
        Name = f.Name,
        Label = f.Label,
        Description = f.Description,
        FieldType = f.FieldType,
        IsPublishedForLookup = f.IsPublishedForLookup,
        IsCollection = f.IsCollection,
        IsLocalized = f.IsLocalized,
        IsNullable = f.IsNullable,
        ReferencedEntityTypeIds = [.. f.ReferencedEntityTypeIds]
      }).ToList()
    };

    try
    {
      if (IsNew)
      {
        var result = await _apiClient.CreateDataModel(_datasource, dto, _cts?.Token ?? CancellationToken.None);
        if (result.IsSuccess)
          await ShowToastSafe(true, "Datový model byl úspěšně vytvořen.");
        else
        {
          var msg = result.Status == Ardalis.Result.ResultStatus.Invalid
            ? string.Join(", ", result.ValidationErrors.Select(e => e.ErrorMessage))
            : string.Join(", ", result.Errors);
          await ShowToastSafe(false, $"Chyba při vytváření: {msg}");
        }
      }
      else
      {
        var result = await _apiClient.UpdateDataModel(_datasource, dto, _cts?.Token ?? CancellationToken.None);
        if (result.IsSuccess)
          await ShowToastSafe(true, "Datový model byl úspěšně uložen.");
        else
        {
          var msg = result.Status == Ardalis.Result.ResultStatus.Invalid
            ? string.Join(", ", result.ValidationErrors.Select(e => e.ErrorMessage))
            : string.Join(", ", result.Errors);
          await ShowToastSafe(false, $"Chyba při ukládání: {msg}");
        }
      }
    }
    catch (OperationCanceledException) { }
    catch (Exception ex)
    {
      Console.WriteLine($"Error in SaveAsync: {ex.Message}");
      await ShowToastSafe(false, "Došlo k neočekávané chybě.");
    }
  }

  private void NavigateBack() => NavigationManager.NavigateTo($"/datamodels/{_datasource.ToString().ToLower()}");

  private async Task ShowToastSafe(bool success, string message)
  {
    if (_disposed || _cts?.Token.IsCancellationRequested == true) return;
    try
    {
      await InvokeAsync(async () =>
      {
        if (!_disposed)
          await ShowToast(success, message);
      });
    }
    catch (Exception ex) { Console.WriteLine($"Toast error: {ex.Message}"); }
  }

  public void Dispose()
  {
    if (!_disposed)
    {
      _disposed = true;
      _cts?.Cancel();
      _cts?.Dispose();
      _cts = null;
    }
  }
}
