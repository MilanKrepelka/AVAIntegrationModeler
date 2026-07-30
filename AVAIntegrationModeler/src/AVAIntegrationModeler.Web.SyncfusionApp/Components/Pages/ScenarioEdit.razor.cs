using System.ComponentModel.DataAnnotations;
using Ardalis.Result;
using AVAIntegrationModeler.API.Client;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Web.SyncfusionApp.Extensions;
using Microsoft.AspNetCore.Components;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Pages;

public partial class ScenarioEdit : ComponentBase, IDisposable
{
  private bool _disposed = false;
  private CancellationTokenSource? _cts;
  
  [Inject] private IAVAIntegrationModelerApiClient _apiClient { get; set; } = default!;

  [Parameter] public string? scenarioCode { get; set; }

  [Parameter] public string dataSourceAsString { get; set; } = string.Empty;

  public Datasource datasource { get; set; }

  DataOperation dataOperation = DataOperation.Update;

  private string? _saveMessage;
  private bool _saveSuccess;

  private ScenarioDTO? _scenarioDTO;
  private List<FeatureDTO> _features = new();

  private class ScenarioEditModel
  {
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Kód je povinný.")]
    [StringLength(100)]
    public string Code { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string NameCz { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string NameEn { get; set; } = string.Empty;

    [StringLength(1000)]
    public string DescriptionCz { get; set; } = string.Empty;

    [StringLength(1000)]
    public string DescriptionEn { get; set; } = string.Empty;

    [Required(ErrorMessage = "GUID je povinný.")]
    [RegularExpression(@"^[0-9A-Fa-f]{8}-([0-9A-Fa-f]{4}-){3}[0-9A-Fa-f]{12}$", ErrorMessage = "Neplatný formát GUID.")]
    public string IdText { get; set; } = string.Empty;

    public Guid? InputFeatureId { get; set; }
    public Guid? OutputFeatureId { get; set; }

    public static ScenarioEditModel NewScenarioEditModel()
    {
      Guid guid = Guid.NewGuid();
      return new ScenarioEditModel()
      {
        Code = string.Empty,
        NameCz = string.Empty,
        NameEn = string.Empty,
        DescriptionCz = string.Empty,
        DescriptionEn = string.Empty,
        IdText = guid.ToString(),
        Id = guid,
        InputFeatureId = null,
        OutputFeatureId = null
      };
    }
  }

  private ScenarioEditModel _edit = new();

  protected override void OnInitialized()
  {
    _cts = new CancellationTokenSource();
  }

  protected async override Task OnParametersSetAsync()
  {
    if (_disposed || _cts?.Token.IsCancellationRequested == true) return;
    
    if (!Enum.TryParse<Datasource>(dataSourceAsString, true, out var datasource))
    {
      datasource = Datasource.Database;
    }
    
    this.datasource = datasource;
    
    try
    {
      var featuresResp = await _apiClient.GetFeatures(datasource, _cts?.Token ?? CancellationToken.None);
      _features = featuresResp.Features ?? new List<FeatureDTO>();

      this.dataOperation = string.IsNullOrEmpty(scenarioCode) ? DataOperation.Create : DataOperation.Update;

      if (string.IsNullOrEmpty(scenarioCode))
      {
        _edit = ScenarioEditModel.NewScenarioEditModel();
      }
      else
      {
        _scenarioDTO = await _apiClient.GetScenario(datasource, scenarioCode, _cts?.Token ?? CancellationToken.None);

        if (_scenarioDTO is not null)
        {
          _edit = new ScenarioEditModel
          {
            Id = _scenarioDTO.Id,
            IdText = _scenarioDTO.Id.ToString(),
            Code = _scenarioDTO.Code,
            NameCz = _scenarioDTO.Name?.CzechValue ?? string.Empty,
            NameEn = _scenarioDTO.Name?.EnglishValue ?? string.Empty,
            DescriptionCz = _scenarioDTO.Description?.CzechValue ?? string.Empty,
            DescriptionEn = _scenarioDTO.Description?.EnglishValue ?? string.Empty,
            InputFeatureId = _scenarioDTO.InputFeatureId,
            OutputFeatureId = _scenarioDTO.OutputFeatureId
          };
        }
      }
    }
    catch (OperationCanceledException)
    {
      // Komponenta byla disposed - ignoruj
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Error in OnParametersSetAsync: {ex.Message}");
    }
  }

  private async Task SaveAsync()
  {
    if (_disposed || _cts?.Token.IsCancellationRequested == true) return;
    
    if (!Guid.TryParse(_edit.IdText, out var parsedId))
    {
      _saveSuccess = false;
      _saveMessage = "Neplatný formát GUID.";
      return;
    }
    
    var updated = new ScenarioDTO
    {
      Id = parsedId == Guid.Empty ? _edit.Id : parsedId,
      Code = _edit.Code,
      Name = new LocalizedValue { CzechValue = _edit.NameCz, EnglishValue = _edit.NameEn },
      Description = new LocalizedValue { CzechValue = _edit.DescriptionCz, EnglishValue = _edit.DescriptionEn },
      InputFeatureId = _edit.InputFeatureId,
      OutputFeatureId = _edit.OutputFeatureId,
      InputFeatureSummary = _edit.InputFeatureId is Guid inId
        ? new FeatureSummaryDTO { Id = inId, Code = _features.FirstOrDefault(f => f.Id == inId)?.Code ?? string.Empty }
        : null,
      OutputFeatureSummary = _edit.OutputFeatureId is Guid outId
        ? new FeatureSummaryDTO { Id = outId, Code = _features.FirstOrDefault(f => f.Id == outId)?.Code ?? string.Empty }
        : null
    };

    _scenarioDTO = updated;
    Result result;

    try
    {
      if (dataOperation == DataOperation.Create)
      {
        var createResult = await _apiClient.CreateScenario(datasource, updated, _cts?.Token ?? CancellationToken.None);
        result = createResult.ToResult();
      }
      else
      {
        var updateResult = await _apiClient.UpdateScenario(datasource, updated, _cts?.Token ?? CancellationToken.None);
        result = updateResult.ToResult();
      }

      if (_disposed || _cts?.Token.IsCancellationRequested == true) return;

      string operationText = dataOperation == DataOperation.Create ? "vytvořen" : "uložen";
      _saveSuccess = result.IsSuccess;
      _saveMessage = result.IsSuccess
        ? $"Integrační scénář {updated.Code} byl v pořádku {operationText}."
        : result.IsInvalid()
          ? string.Join(", ", result.ValidationErrors.Select(e => e.ErrorMessage))
          : string.Join(", ", result.Errors);
    }
    catch (OperationCanceledException)
    {
      Console.WriteLine("SaveAsync was cancelled");
    }
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

