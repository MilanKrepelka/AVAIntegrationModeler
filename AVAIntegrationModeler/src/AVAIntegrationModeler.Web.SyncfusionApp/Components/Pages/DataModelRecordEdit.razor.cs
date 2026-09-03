using System.ComponentModel.DataAnnotations;
using Ardalis.Result;
using AVAIntegrationModeler.API.Client;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.AspNetCore.Components;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Pages;

public partial class DataModelRecordEdit : ComponentBase, IDisposable
{
  private bool _disposed = false;
  private CancellationTokenSource? _cts;
  private readonly Guid _newRecordId = Guid.NewGuid();

  [Inject] private IAVAIntegrationModelerApiClient _apiClient { get; set; } = default!;
  [Inject] private NavigationManager NavigationManager { get; set; } = default!;

  [Parameter] public string dataSourceAsString { get; set; } = string.Empty;
  [Parameter] public Guid modelId { get; set; }
  [Parameter] public Guid recordId { get; set; }

  private Datasource _datasource;
  private DataModelDTO? _dataModel;
  private DataModelRecordDTO? _existingRecord;
  private string? _saveMessage;
  private bool _saveSuccess;

  public bool IsLoading { get; set; } = true;
  public bool IsCreate => recordId == Guid.Empty;

  private class RecordEditModel
  {
    [StringLength(500)]
    public string ExternalId { get; set; } = string.Empty;
  }

  private class FieldValueModel
  {
    public bool IsLocalized { get; set; }
    public string? StringValue { get; set; }
    public string? CzechValue { get; set; }
    public string? EnglishValue { get; set; }
  }

  private RecordEditModel _edit = new();
  private Dictionary<string, FieldValueModel> _fieldValues = new();

  protected override void OnInitialized()
  {
    _cts = new CancellationTokenSource();
  }

  protected override async Task OnParametersSetAsync()
  {
    if (_disposed || _cts?.Token.IsCancellationRequested == true) return;

    if (!Enum.TryParse<Datasource>(dataSourceAsString, true, out var ds))
      ds = Datasource.Database;
    _datasource = ds;

    IsLoading = true;

    try
    {
      var ct = _cts?.Token ?? CancellationToken.None;

      _dataModel = await _apiClient.GetDataModel(_datasource, modelId, ct);

      _fieldValues.Clear();
      if (_dataModel is not null)
      {
        foreach (var field in _dataModel.Fields)
        {
          _fieldValues[field.Name] = new FieldValueModel { IsLocalized = field.IsLocalized };
        }
      }

      if (IsCreate)
      {
        GenerateExternalId();
      }
      else
      {
        _existingRecord = await _apiClient.GetDataModelRecord(_datasource, recordId, ct);
        if (_existingRecord is not null)
        {
          _edit.ExternalId = _existingRecord.ExternalId;
          foreach (var f in _existingRecord.Fields)
          {
            if (_fieldValues.TryGetValue(f.Key, out var fv))
            {
              fv.IsLocalized = f.IsLocalized;
              fv.StringValue = f.StringValue;
              fv.CzechValue = f.CzechValue;
              fv.EnglishValue = f.EnglishValue;
            }
          }
        }
      }
    }
    catch (OperationCanceledException) { }
    catch (Exception ex)
    {
      Console.WriteLine($"Error in OnParametersSetAsync: {ex.Message}");
    }
    finally
    {
      IsLoading = false;
    }
  }

  private async Task SaveAsync()
  {
    if (_disposed || _cts?.Token.IsCancellationRequested == true) return;

    var ct = _cts?.Token ?? CancellationToken.None;

    var fields = _fieldValues.Select(kvp => new DataModelRecordFieldDTO
    {
      Key = kvp.Key,
      IsLocalized = kvp.Value.IsLocalized,
      StringValue = kvp.Value.IsLocalized ? null : kvp.Value.StringValue,
      CzechValue = kvp.Value.IsLocalized ? kvp.Value.CzechValue : null,
      EnglishValue = kvp.Value.IsLocalized ? kvp.Value.EnglishValue : null
    }).ToList();

    var record = new DataModelRecordDTO
    {
      Id = IsCreate ? _newRecordId : recordId,
      ModelId = modelId,
      ExternalId = _edit.ExternalId,
      Fields = fields
    };

    try
    {
      Result<Guid> result;
      if (IsCreate)
        result = await _apiClient.CreateDataModelRecord(_datasource, record, ct);
      else
        result = await _apiClient.UpdateDataModelRecord(_datasource, record, ct);

      if (_disposed) return;

      if (result.IsSuccess)
      {
        _saveSuccess = true;
        _saveMessage = $"Záznam byl v pořádku {(IsCreate ? "vytvořen" : "uložen")}.";
      }
      else
      {
        _saveSuccess = false;
        _saveMessage = result.IsInvalid()
          ? string.Join(", ", result.ValidationErrors.Select(e => e.ErrorMessage))
          : string.Join(", ", result.Errors);
      }
    }
    catch (OperationCanceledException) { }
    catch (Exception ex)
    {
      _saveSuccess = false;
      _saveMessage = "Došlo k neočekávané chybě.";
      Console.WriteLine($"SaveAsync error: {ex.Message}");
    }
  }

  private void GenerateExternalId()
  {
    var codeValue = _fieldValues.TryGetValue("Code", out var fv) ? fv.StringValue : null;
    var parts = new List<string>();
    if (!string.IsNullOrWhiteSpace(codeValue))
      parts.Add(codeValue);
    if (!string.IsNullOrWhiteSpace(_dataModel?.Code))
      parts.Add(_dataModel.Code);
    parts.Add(Guid.Empty.ToString());
    _edit.ExternalId = string.Join(".", parts);
  }

  private void HandleFieldValueChanged(string fieldName, string value)
  {
    if (_fieldValues.TryGetValue(fieldName, out var fv))
      fv.StringValue = value;
    if (fieldName == "Code" && IsCreate)
      GenerateExternalId();
  }

  private void GoBack()
  {
    NavigationManager.NavigateTo($"/datamodelrecords/{_datasource.ToString().ToLower()}");
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
