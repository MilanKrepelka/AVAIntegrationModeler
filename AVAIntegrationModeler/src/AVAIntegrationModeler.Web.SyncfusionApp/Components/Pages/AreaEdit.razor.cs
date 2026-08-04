using System.ComponentModel.DataAnnotations;
using Ardalis.Result;
using AVAIntegrationModeler.API.Client;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.AspNetCore.Components;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Pages;

/// <summary>
/// Stránka pro vytvoření a editaci oblasti.
/// </summary>
public partial class AreaEdit : ComponentBase, IDisposable
{
  private bool _disposed = false;
  private CancellationTokenSource? _cts;

  [Inject] private IAVAIntegrationModelerApiClient ApiClient { get; set; } = default!;
  [Inject] private NavigationManager NavigationManager { get; set; } = default!;

  [Parameter] public string? areaCode { get; set; }

  private bool IsEditMode => !string.IsNullOrEmpty(areaCode);

  private class AreaEditModel
  {
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Kód oblasti je povinný.")]
    [StringLength(100)]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Název oblasti je povinný.")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public string IdText { get; set; } = string.Empty;

    public static AreaEditModel New()
    {
      var id = Guid.NewGuid();
      return new AreaEditModel { Id = id, IdText = id.ToString() };
    }
  }

  private AreaEditModel _edit = AreaEditModel.New();

  protected override void OnInitialized()
  {
    _cts = new CancellationTokenSource();
  }

  protected override async Task OnParametersSetAsync()
  {
    if (_disposed || _cts?.Token.IsCancellationRequested == true) return;

    try
    {
      if (string.IsNullOrEmpty(areaCode))
      {
        _edit = AreaEditModel.New();
      }
      else
      {
        var dto = await ApiClient.GetArea(Datasource.Database, areaCode, _cts?.Token ?? CancellationToken.None);
        if (dto != null)
        {
          _edit = new AreaEditModel
          {
            Id = dto.Id,
            IdText = dto.Id.ToString(),
            Code = dto.Code,
            Name = dto.Name
          };
        }
      }
    }
    catch (OperationCanceledException) { }
    catch (Exception ex)
    {
      Console.WriteLine($"Chyba při načítání oblasti: {ex.Message}");
    }
  }

  private string? _saveMessage;
  private bool _saveSuccess;

  private async Task SaveAsync()
  {
    if (_disposed || _cts?.Token.IsCancellationRequested == true) return;

    var dto = new AreaDTO
    {
      Id = _edit.Id,
      Code = _edit.Code,
      Name = _edit.Name
    };

    try
    {
      if (IsEditMode)
      {
        var result = await ApiClient.UpdateArea(Datasource.Database, dto, _cts?.Token ?? CancellationToken.None);
        if (_disposed) return;
        _saveSuccess = result.IsSuccess;
        _saveMessage = result.IsSuccess
          ? $"Oblast {dto.Code} byla uložena."
          : result.IsInvalid()
            ? string.Join(", ", result.ValidationErrors.Select(e => e.ErrorMessage))
            : string.Join(", ", result.Errors);
      }
      else
      {
        var result = await ApiClient.CreateArea(Datasource.Database, dto, _cts?.Token ?? CancellationToken.None);
        if (_disposed) return;
        _saveSuccess = result.IsSuccess;
        _saveMessage = result.IsSuccess
          ? $"Oblast {dto.Code} byla vytvořena."
          : result.IsInvalid()
            ? string.Join(", ", result.ValidationErrors.Select(e => e.ErrorMessage))
            : string.Join(", ", result.Errors);
      }
    }
    catch (OperationCanceledException) { }
    catch (Exception ex)
    {
      Console.WriteLine($"Chyba při ukládání oblasti: {ex.Message}");
      if (!_disposed)
      {
        _saveSuccess = false;
        _saveMessage = "Došlo k neočekávané chybě.";
      }
    }
  }

  private void GoBack()
  {
    NavigationManager.NavigateTo("/areas");
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
