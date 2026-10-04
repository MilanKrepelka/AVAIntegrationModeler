using System.ComponentModel.DataAnnotations;
using Ardalis.Result;
using AVAIntegrationModeler.API.Client;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Localization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

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
  [Inject] private IJSRuntime JS { get; set; } = default!;

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

    public DateTime? LastSaveDateTime { get; set; }

    public DateTime? LastDeploymentDateTime { get; set; }

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

  private string? _avaPlaceMessage;
  private bool _avaPlaceSuccess;
  private bool _avaPlaceLoading;

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
      _availableDataModels = modelsResponse?.DataModels?.OrderBy(item=>item.Code)?.Select(m => new DataModelDTO
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
            DataModelIds = dto.DataModelIds.ToList(),
            LastSaveDateTime = dto.LastSaveDateTime,
            LastDeploymentDateTime = dto.LastDeploymentDateTime
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
          ? string.Format(SharedResources.Deployments_Saved, dto.Code)
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
          ? string.Format(SharedResources.Deployments_Created, dto.Code)
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
        _saveMessage = SharedResources.Common_UnexpectedError;
      }
    }
  }

  private void GoBack()
  {
    NavigationManager.NavigateTo("/deployments");
  }

  /// <summary>
  /// Stáhne ZIP s exportem DataModelů a jejich záznamů pro toto nasazení.
  /// </summary>
  private async Task ExportAsync()
  {
    if (string.IsNullOrEmpty(deploymentCode)) return;
    try
    {
      var bytes = await ApiClient.ExportDeployment(deploymentCode, _cts?.Token ?? CancellationToken.None);
      await JS.InvokeVoidAsync("downloadFile", $"export-{deploymentCode}.zip", "application/zip", bytes);
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Chyba při exportu nasazení: {ex.Message}");
    }
  }

  /// <summary>
  /// Nahraje DataModely a záznamy nasazení do AVAPlace.
  /// </summary>
  private async Task UploadToAvaPlaceAsync()
  {
    if (string.IsNullOrEmpty(deploymentCode)) return;
    _avaPlaceLoading = true;
    _avaPlaceMessage = null;
    await InvokeAsync(StateHasChanged);
    try
    {
      var result = await ApiClient.UploadDeploymentToAvaPlace(deploymentCode, _cts?.Token ?? CancellationToken.None);
      if (_disposed) return;
      if (result.IsSuccess)
      {
        var v = result.Value;
        var hasErrors = v.Errors.Count > 0;
        _avaPlaceSuccess = !hasErrors;
        _avaPlaceMessage = hasErrors
          ? $"Import dokončen s chybami. Verze: {v.VersionCode}, modely: {v.ModelsImported}, záznamy: {v.RecordGroupsImported}.\n{string.Join("\n", v.Errors)}"
          : $"Import do AVAPlace proběhl úspěšně. Verze: {v.VersionCode}, modely: {v.ModelsImported}, záznamy: {v.RecordGroupsImported}.";
      }
      else
      {
        _avaPlaceSuccess = false;
        _avaPlaceMessage = string.Join(", ", result.Errors);
      }
    }
    catch (Exception ex)
    {
      if (!_disposed)
      {
        _avaPlaceSuccess = false;
        _avaPlaceMessage = $"Chyba při nahrávání do AVAPlace: {ex.Message}";
      }
    }
    finally
    {
      _avaPlaceLoading = false;
      await InvokeAsync(StateHasChanged);
    }
  }

  private bool _docsExportLoading;
  private string? _docsExportError;

  /// <summary>
  /// Stáhne ZIP s markdown dokumentací nasazení.
  /// </summary>
  private async Task ExportDocumentationAsync()
  {
    if (string.IsNullOrEmpty(deploymentCode)) return;
    _docsExportLoading = true;
    _docsExportError = null;
    await InvokeAsync(StateHasChanged);
    try
    {
      var bytes = await ApiClient.ExportDeploymentDocumentation(deploymentCode, cancellationToken: _cts?.Token ?? CancellationToken.None);
      await JS.InvokeVoidAsync("downloadFile", $"deployment-{deploymentCode}-docs.zip", "application/zip", bytes);
    }
    catch (Exception ex)
    {
      if (!_disposed)
        _docsExportError = ex.Message;
    }
    finally
    {
      _docsExportLoading = false;
      await InvokeAsync(StateHasChanged);
    }
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

  #region Wizard vývoje

  /// <summary>
  /// Položka textu kroku wizardu — volitelně s odkazem.
  /// </summary>
  /// <param name="Text">Text pokynu.</param>
  /// <param name="Url">Volitelný odkaz zobrazený za textem.</param>
  private sealed record WizardInstruction(string Text, string? Url = null);

  /// <summary>
  /// Krok wizardu vývoje — nadpis a seznam pokynů.
  /// </summary>
  /// <param name="Title">Nadpis kroku.</param>
  /// <param name="Instructions">Pokyny zobrazené v kroku.</param>
  private sealed record WizardStep(string Title, IReadOnlyList<WizardInstruction> Instructions);

  /// <summary>
  /// Definice kroků wizardu vývoje. Pořadí odpovídá indexu <see cref="_wizardStep"/>,
  /// tlačítka akcí jednotlivých kroků jsou v šabloně podle indexu.
  /// </summary>
  private static readonly IReadOnlyList<WizardStep> WizardSteps =
  [
    new("Vytvoření vývojových větví",
    [
      new("Vytvoř pomocí skriptu PrepareFolders.cmd adresáře pro vývoj.")
    ]),
    new("Export modelů",
    [
      new("Exportuj modely a unified data do /IntegrationsDataModels/IntegrationsDataModels."),
      new("Pomocí AI zkontroluj modely na logiku."),
      new("Pomocí AI ověř, jestli v modelech nejsou překlepy."),
      new("Pomocí AI ověř, jestli v unified data nejsou překlepy."),
      new("V případě chyb oprav a exportuj znovu."),
      new("Ověř rozdíly oproti databázi, jestli tam je všechno.")
    ]),
    new("Export dokumentace",
    [
      new("Nahraj modely a unified data do AVAPlace."),
      new("Exportuj dokumentaci do /VDM/VDM."),
      new("Commitni do aktuální větve pro vývoj."),
      new("Udělej pull request do draft větve."),
      new("Nech ověřit analytikem.")
    ]),
    new("Vytvoření pull requestů pro architekta",
    [
      new("Pokud jde o nové modely, nezapomeň dát patřičné řádky do globálních skriptů."),
      new("Vytvoř pull request pro modely:", "https://asolcz.visualstudio.com/Plaza/_git/IntegrationDataModels/branches"),
      new("Vytvoř pull request pro dokumentaci:", "https://asolcz.visualstudio.com/Docs/_git/VDM/branches")
    ]),
    new("Předání výsledku architektovi",
    [
      new("Přehoď vývojový tiket(y) na architekta.")
    ])
  ];

  private bool _wizardActive;
  private int _wizardStep;

  private string? _completeMessage;
  private bool _completeSuccess;
  private bool _completeLoading;

  private bool IsFirstWizardStep => _wizardStep == 0;
  private bool IsLastWizardStep => _wizardStep == WizardSteps.Count - 1;

  /// <summary>
  /// Spustí wizard vývoje od prvního kroku.
  /// </summary>
  private void StartWizard()
  {
    _wizardActive = true;
    _wizardStep = 0;
    _completeMessage = null;
  }

  /// <summary>
  /// Zavře wizard vývoje.
  /// </summary>
  private void CloseWizard() => _wizardActive = false;

  /// <summary>
  /// Přejde na další krok wizardu.
  /// </summary>
  private void NextWizardStep()
  {
    if (!IsLastWizardStep) _wizardStep++;
  }

  /// <summary>
  /// Vrátí se na předchozí krok wizardu.
  /// </summary>
  private void PreviousWizardStep()
  {
    if (!IsFirstWizardStep) _wizardStep--;
  }

  /// <summary>
  /// Přejde přímo na zvolený krok wizardu.
  /// </summary>
  /// <param name="step">Index kroku.</param>
  private void GoToWizardStep(int step)
  {
    if (step >= 0 && step < WizardSteps.Count) _wizardStep = step;
  }

  /// <summary>
  /// Ukončí vývoj — uloží aktuální stav formuláře a nastaví datum posledního nasazení na aktuální čas.
  /// </summary>
  private async Task CompleteDevelopmentAsync()
  {
    if (string.IsNullOrEmpty(deploymentCode) || _disposed) return;
    _completeLoading = true;
    _completeMessage = null;
    await InvokeAsync(StateHasChanged);
    try
    {
      var ct = _cts?.Token ?? CancellationToken.None;

      // Nejprve uložit případné neuložené změny formuláře, aby se neztratily.
      await SaveAsync();
      if (_disposed) return;
      if (!_saveSuccess)
      {
        _completeSuccess = false;
        _completeMessage = $"Vývoj nebyl ukončen — nasazení se nepodařilo uložit: {_saveMessage}";
        return;
      }

      var result = await ApiClient.CompleteDeploymentDevelopment(_edit.Code, ct);
      if (_disposed) return;
      _completeSuccess = result.IsSuccess;
      if (result.IsSuccess)
      {
        _edit.LastDeploymentDateTime = result.Value.LastDeploymentDateTime;
        _edit.LastSaveDateTime = result.Value.LastSaveDateTime;
        _completeMessage = $"Vývoj nasazení {_edit.Code} byl ukončen.";
      }
      else
      {
        _completeMessage = result.IsInvalid()
          ? string.Join(", ", result.ValidationErrors.Select(e => e.ErrorMessage))
          : string.Join(", ", result.Errors);
      }
    }
    catch (OperationCanceledException) { }
    catch (Exception ex)
    {
      if (!_disposed)
      {
        _completeSuccess = false;
        _completeMessage = $"Chyba při ukončení vývoje: {ex.Message}";
      }
    }
    finally
    {
      _completeLoading = false;
      if (!_disposed)
        await InvokeAsync(StateHasChanged);
    }
  }

  #endregion

  private void NavigateToModelChanges(Guid modelId)
    => NavigationManager.NavigateTo($"/datamodelchanges/{modelId}?returnUrl=/deploymentedit/{deploymentCode}");

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
