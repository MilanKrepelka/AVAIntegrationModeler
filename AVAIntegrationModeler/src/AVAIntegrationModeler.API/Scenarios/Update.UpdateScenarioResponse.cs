using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.API.Scenarios;

public class UpdateScenarioResponse(ScenarioDTO scenario)
{
  public ScenarioDTO Scenario { get; set; } = scenario;
}
