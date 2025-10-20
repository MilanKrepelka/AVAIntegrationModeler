using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.API.Scenarios;

/// <summary>
/// Odpověď na požadavek <see cref="ScenarioListRequest"/>
/// </summary>
public class ScenarioListResponse
{
  /// <summary>
  /// Seznam <see cref="ScenarioDTO"/>
  /// </summary>
  public List<ScenarioDTO> Scenarios { get; set; } = [];
}
