using System.ComponentModel.DataAnnotations;
using AVAIntegrationModeler.API.Client;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Inputs.Internal;
namespace AVAIntegrationModeler.Web.SyncfusionApp.Components.Pages;

public partial class ScenarioEdit : ComponentBase
{
  [Inject] private IAVAIntegrationModelerApiClient _apiClient { get; set; } = default!;
  [Parameter] public string scenarioCode { get; set; } = string.Empty;

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

    // GUID jako text pro bind + validace
    [Required(ErrorMessage = "GUID je povinný.")]
    [RegularExpression(@"^[0-9A-Fa-f]{8}-([0-9A-Fa-f]{4}-){3}[0-9A-Fa-f]{12}$", ErrorMessage = "Neplatný formát GUID.")]
    public string IdText { get; set; } = string.Empty;

    public Guid? InputFeatureId { get; set; }
    public Guid? OutputFeatureId { get; set; }
  }

  private ScenarioEditModel _edit = new();

  protected override async Task OnInitializedAsync()
  {
    if (string.IsNullOrWhiteSpace(scenarioCode))
      return;

    _scenarioDTO = await _apiClient.GetScenario(Datasource.AVAPlace, scenarioCode, CancellationToken.None);
    var featuresResp = await _apiClient.GetFeatures(Datasource.AVAPlace, CancellationToken.None);
    _features = featuresResp.Features ?? new List<FeatureDTO>();

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

  private async Task SaveAsync()
  {
    if (_scenarioDTO is null) return;

    // Převod IdText -> Id (ochrana před neplatným GUID už přes DataAnnotations)
    _ = Guid.TryParse(_edit.IdText, out var parsedId);
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
    await InvokeAsync(StateHasChanged);
  }

 
  private void Cancel()
  {
    if (_scenarioDTO is null) return;

    _edit.Code = _scenarioDTO.Code;
    _edit.IdText = _scenarioDTO.Id.ToString();
    _edit.NameCz = _scenarioDTO.Name?.CzechValue ?? string.Empty;
    _edit.NameEn = _scenarioDTO.Name?.EnglishValue ?? string.Empty;
    _edit.DescriptionCz = _scenarioDTO.Description?.CzechValue ?? string.Empty;
    _edit.DescriptionEn = _scenarioDTO.Description?.EnglishValue ?? string.Empty;
    _edit.InputFeatureId = _scenarioDTO.InputFeatureId;
    _edit.OutputFeatureId = _scenarioDTO.OutputFeatureId;
  }
}
