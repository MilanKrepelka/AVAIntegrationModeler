using AVAIntegrationModeler.API.Client;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.AspNetCore.Components;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Pages;

/// <summary>
/// Stránka pro zobrazení (vizualizaci) záznamu datového modelu — pouze pro čtení.
/// </summary>
public partial class DataModelRecordView : ComponentBase, IDisposable
{
  private bool _disposed = false;
  private CancellationTokenSource? _cts;

  [Inject] private IAVAIntegrationModelerApiClient _apiClient { get; set; } = default!;
  [Inject] private NavigationManager NavigationManager { get; set; } = default!;

  [Parameter] public string dataSourceAsString { get; set; } = string.Empty;
  [Parameter] public Guid modelId { get; set; }
  [Parameter] public Guid recordId { get; set; }

  [SupplyParameterFromQuery(Name = "returnUrl")] public string? ReturnUrl { get; set; }

  private Datasource _datasource;
  private DataModelDTO? _dataModel;

  private class FieldValueModel
  {
    public bool IsLocalized { get; set; }
    public string? StringValue { get; set; }
    public string? CzechValue { get; set; }
    public string? EnglishValue { get; set; }
  }

  private string _externalId = string.Empty;
  private Dictionary<string, FieldValueModel> _fieldValues = new();

  public bool IsLoading { get; set; } = true;

  protected override void OnInitialized()
  {
    _cts = new CancellationTokenSource();
  }

  protected override async Task OnParametersSetAsync()
  {
    if (_disposed || _cts?.Token.IsCancellationRequested == true) return;

    if (!Enum.TryParse<Datasource>(dataSourceAsString, true, out var ds))
      ds = Datasource.AVAPlace;
    _datasource = ds;

    IsLoading = true;

    try
    {
      var ct = _cts?.Token ?? CancellationToken.None;

      _dataModel = await _apiClient.GetDataModel(_datasource, modelId, ct);

      _fieldValues.Clear();
      if (_dataModel is not null)
        foreach (var field in _dataModel.Fields)
          _fieldValues[field.Name] = new FieldValueModel { IsLocalized = field.IsLocalized };

      var record = await _apiClient.GetDataModelRecord(_datasource, recordId, ct);
      if (record is not null)
      {
        _externalId = record.ExternalId;
        foreach (var f in record.Fields)
        {
          if (_fieldValues.TryGetValue(f.Key, out var fv))
          {
            fv.IsLocalized = f.IsLocalized;
            fv.StringValue = f.StringValue;
            fv.CzechValue = f.CzechValue;
            fv.EnglishValue = f.EnglishValue;
          }
          else
          {
            // pole existuje v záznamu, ale ne ve schématu — zobrazíme ho taky
            _fieldValues[f.Key] = new FieldValueModel
            {
              IsLocalized = f.IsLocalized,
              StringValue = f.StringValue,
              CzechValue = f.CzechValue,
              EnglishValue = f.EnglishValue,
            };
          }
        }
      }
    }
    catch (OperationCanceledException) { }
    catch (Exception ex)
    {
      Console.WriteLine($"DataModelRecordView: {ex.Message}");
    }
    finally
    {
      IsLoading = false;
    }
  }

  private void GoBack()
  {
    if (!string.IsNullOrEmpty(ReturnUrl))
      NavigationManager.NavigateTo(ReturnUrl);
    else
      NavigationManager.NavigateTo($"/datamodelrecords/{dataSourceAsString.ToLower()}");
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
