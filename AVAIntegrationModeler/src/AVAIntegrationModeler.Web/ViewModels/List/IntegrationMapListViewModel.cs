using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Web.ViewModels.List;

/// <summary>
/// Třída představující ViewModel pro seznam scénářů.
/// </summary>
public class IntegrationMapListViewModel
{
  /// <summary>
  /// Oblast
  /// </summary>
  public string Area { get; init; } = string.Empty;

  public string ScenarioCodes { get; init; } = string.Empty;

}
