using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Scenarios;

namespace AVAIntegrationModeler.API.Scenarios.ViewModels;

/// <summary>
/// View model pro Scenario s plným náhledem
/// </summary>
public record ScenarioFullPreview : ScenarioRecord
{
  public string InputFeatureCode { get; set; } = string.Empty;
  public string OutputFeatureCode { get; set; } = string.Empty;

  public ScenarioFullPreview()
  {
  }

  public ScenarioFullPreview(ScenarioDTO original) : base(original)
  {
  }
}
