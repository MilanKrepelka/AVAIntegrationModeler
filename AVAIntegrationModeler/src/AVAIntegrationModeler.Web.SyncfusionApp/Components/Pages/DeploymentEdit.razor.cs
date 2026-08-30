using System.ComponentModel.DataAnnotations;
using Ardalis.Result;
using AVAIntegrationModeler.API.Client;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.AspNetCore.Components;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Pages;

/// <summary>
/// Stránka pro vytvoření a editaci nasazení.
/// </summary>
public partial class DeploymentEdit : ComponentBase, IDisposable
{
  private bool _disposed = false;
  private CancellationTokenSource? _cts;

  [Inject] private IAVAIntegrationModelerApiClient ApiClient { get; set; } = default!;
  [Inject] private NavigationManager NavigationManager { get; set; } = default!;

  [Parameter] public string? deploymentCode { get; set; }

  private bool IsEditMode => !string.IsNullOrEmpty(deploymentCode);

  private class DeploymentEditModel
  {
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Kód nasazení je povinný.")]
    [StringLength(100)]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Název nasazení je povinný.")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Ticket { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    public string IdText { get; set; } = string.Empty;

    public List<Guid> DataModelIds { get; set; } = new();

    public static DeploymentEditModel New()
    {
      var id = Guid.NewGuid();
      return new DeploymentEditModel { Id = id, IdText = id.ToString() };
    }
  }

  private DeploymentEditModel _edit = DeploymentEditModel.New();
  private List<DataModelDTO> _availableDataModels = new();
  private List<DataModelDTO> _selectableDataModels = new();
  private Guid _selectedDataModelId = Guid.Empty;

  private string? _saveMessage;
  private bool _saveSuccess;

  protected override void OnInitialized()
  {
    _cts = new CancellationTokenSource();
  }

  protected override async Task OnParametersSetAsync()
  {
    if (_disposed || _cts?.Token.IsCancellationRequested == true) return;

    try
    {
      var modelsResponse = await ApiClient.GetDataModels(Datasource.Database, _cts?.Token ?? CancellationToken.None);
      _availableDataModels = modelsResponse?.DataModels?.Select(m => new DataModelDTO
      {
        Id = m.Id, Code = m.Code, Name = m.Name
      }).ToList() ?? new();

      if (string.IsNullOrEmpty(deploymentCode))
      {
        _edit = DeploymentEditModel.New();
      }
      else
      {
        var dto = await ApiClient.GetDeployment(deploymentCode, _cts?.Token ?? CancellationToken.None);
        if (dto != null)
        {
          _edit = new DeploymentEditModel
          {
            Id = dto.Id,
            IdText = dto.Id.ToString(),
            Code = dto.Code,
            Name = dto.Name,
            Ticket = dto.Ticket,
            Description = dto.Description,
            DataModelIds = dto.DataModelIds.ToList()
          };
        }
      }

      UpdateSelectableDataModels();
    }
    catch (OperationCanceledException) { }
    catch (Exception ex)
    {
      Console.WriteLine($"Chyba při načítání nasazení: {ex.Message}");
    }
  }

  private void UpdateSelectableDataModels()
  {
    _selectableDataModels = _availableDataModels
      .Where(m => !_edit.DataModelIds.Contains(m.Id))
      .ToList();
    _selectedDataModelId = _selectableDataModels.FirstOrDefault()?.Id ?? Guid.Empty;
  }

  private void AddDataModel()
  {
    if (_selectedDataModelId == Guid.Empty) return;
    if (!_edit.DataModelIds.Contains(_selectedDataModelId))
      _edit.DataModelIds.Add(_selectedDataModelId);
    UpdateSelectableDataModels();
  }

  private void RemoveDataModel(Guid id)
  {
    _edit.DataModelIds.Remove(id);
    UpdateSelectableDataModels();
  }

  private async Task SaveAsync()
  {
    if (_disposed || _cts?.Token.IsCancellationRequested == true) return;

    var dto = new DeploymentDTO
    {
      Id = _edit.Id,
      Code = _edit.Code,
      Name = _edit.Name,
      Ticket = string.IsNullOrWhiteSpace(_edit.Ticket) ? null : _edit.Ticket.Trim(),
      Description = string.IsNullOrWhiteSpace(_edit.Description) ? null : _edit.Description.Trim(),
      DataModelIds = _edit.DataModelIds.ToList()
    };

    try
    {
      if (IsEditMode)
      {
        var result = await ApiClient.UpdateDeployment(dto, _cts?.Token ?? CancellationToken.None);
        if (_disposed) return;
        _saveSuccess = result.IsSuccess;
        _saveMessage = result.IsSuccess
          ? $"Nasazení {dto.Code} bylo uloženo."
          : result.IsInvalid()
            ? string.Join(", ", result.ValidationErrors.Select(e => e.ErrorMessage))
            : string.Join(", ", result.Errors);
      }
      else
      {
        var result = await ApiClient.CreateDeployment(dto, _cts?.Token ?? CancellationToken.None);
        if (_disposed) return;
        _saveSuccess = result.IsSuccess;
        _saveMessage = result.IsSuccess
          ? $"Nasazení {dto.Code} bylo vytvořeno."
          : result.IsInvalid()
            ? string.Join(", ", result.ValidationErrors.Select(e => e.ErrorMessage))
            : string.Join(", ", result.Errors);
      }
    }
    catch (OperationCanceledException) { }
    catch (Exception ex)
    {
      Console.WriteLine($"Chyba při ukládání nasazení: {ex.Message}");
      if (!_disposed)
      {
        _saveSuccess = false;
        _saveMessage = "Došlo k neočekávané chybě.";
      }
    }
  }

  private void GoBack()
  {
    NavigationManager.NavigateTo("/deployments");
  }

  private Contracts.DataModels.DeploymentChangesSummaryDTO? _changesSummary;
  private bool _loadingSummary;
  private string? _summaryError;

  /// <summary>
  /// Načte souhrnné porovnání DataModelů nasazení.
  /// </summary>
  private async Task LoadChangesSummaryAsync()
  {
    if (string.IsNullOrEmpty(deploymentCode)) return;
    _loadingSummary = true;
    _changesSummary = null;
    _summaryError = null;
    await InvokeAsync(StateHasChanged);
    try
    {
      _changesSummary = await ApiClient.GetDeploymentChangesSummary(
        _edit.Code, _edit.Name, _edit.DataModelIds, _cts?.Token ?? CancellationToken.None);
      if (_changesSummary is null)
        _summaryError = "Nepodařilo se načíst přehled změn.";
    }
    catch (Exception ex)
    {
      _summaryError = $"Chyba: {ex.Message}";
    }
    finally
    {
      _loadingSummary = false;
      await InvokeAsync(StateHasChanged);
    }
  }

  private void NavigateToModelChanges(Guid modelId)
    => NavigationManager.NavigateTo($"/datamodelchanges/{modelId}");

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
