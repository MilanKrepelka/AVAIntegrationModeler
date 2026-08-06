using System.ComponentModel.DataAnnotations;
using Ardalis.Result;
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

  private string? _saveMessage;
  private bool _saveSuccess;

  private List<DataModelFieldEditModel> _fields = [];
  private List<DataModelSummaryDTO> _availableModels = [];
  private List<AreaDTO> _availableAreas = [];

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

    public Guid? AreaId { get; set; }
  }

  private DataModelEditModel _edit = new();

  /// <summary>
  /// Nastaví se na true po prvním úspěšném Create — další automatické uložení
  /// (např. po přidání dalšího pole) tak už jde přes Update, ne přes duplicitní Create.
  /// </summary>
  private bool _hasBeenCreated;

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
          IsAggregateRoot = dto.IsAggregateRoot,
          AreaId = dto.AreaId
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

    try
    {
      var areasResponse = await _apiClient.GetAreas(Datasource.Database, token);
      _availableAreas = areasResponse?.Areas?.OrderBy(a => a.Name).ToList() ?? [];
    }
    catch (Exception ex) { Console.WriteLine($"Error loading areas: {ex.Message}"); }
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
      AreaId = _edit.AreaId,
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
      if (IsNew && !_hasBeenCreated)
      {
        var result = await _apiClient.CreateDataModel(_datasource, dto, _cts?.Token ?? CancellationToken.None);
        if (_disposed) return;
        _saveSuccess = result.IsSuccess;
        if (result.IsSuccess)
          _hasBeenCreated = true;
        _saveMessage = result.IsSuccess
          ? $"Datový model {dto.Code} byl vytvořen."
          : result.IsInvalid()
            ? string.Join(", ", result.ValidationErrors.Select(e => e.ErrorMessage))
            : string.Join(", ", result.Errors);
      }
      else
      {
        var result = await _apiClient.UpdateDataModel(_datasource, dto, _cts?.Token ?? CancellationToken.None);
        if (_disposed) return;
        _saveSuccess = result.IsSuccess;
        _saveMessage = result.IsSuccess
          ? $"Datový model {dto.Code} byl uložen."
          : result.IsInvalid()
            ? string.Join(", ", result.ValidationErrors.Select(e => e.ErrorMessage))
            : string.Join(", ", result.Errors);
      }
    }
    catch (OperationCanceledException) { }
    catch (Exception ex)
    {
      Console.WriteLine($"Error in SaveAsync: {ex.Message}");
      if (!_disposed)
      {
        _saveSuccess = false;
        _saveMessage = "Došlo k neočekávané chybě.";
      }
    }
  }

  private void NavigateBack()
  {
    NavigationManager.NavigateTo($"/datamodels/{_datasource.ToString().ToLower()}");
  }

  public void Dispose()
  {
    if (_disposed) return;
    _disposed = true;
    _cts?.Cancel();
    _cts?.Dispose();
    _cts = null;
  }
}
