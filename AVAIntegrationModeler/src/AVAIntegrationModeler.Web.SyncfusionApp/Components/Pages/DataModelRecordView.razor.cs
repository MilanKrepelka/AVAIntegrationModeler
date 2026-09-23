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
  private DataModelRecordDTO? _record;

  public bool IsLoading { get; set; } = true;
  public string? ErrorMessage { get; set; }

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
    ErrorMessage = null;

    try
    {
      var ct = _cts?.Token ?? CancellationToken.None;

      _dataModel = await _apiClient.GetDataModel(_datasource, modelId, ct);
      _record = await _apiClient.GetDataModelRecord(_datasource, recordId, ct);
    }
    catch (OperationCanceledException) { }
    catch (Exception ex)
    {
      ErrorMessage = ex.Message;
    }
    finally
    {
      IsLoading = false;
    }
  }

  /// <summary>
  /// Vrátí hodnotu pole záznamu podle klíče.
  /// </summary>
  private DataModelRecordFieldDTO? GetField(string key)
    => _record?.Fields.FirstOrDefault(f => string.Equals(f.Key, key, StringComparison.OrdinalIgnoreCase));

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
