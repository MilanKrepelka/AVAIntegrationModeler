using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.API.Scenarios;

public class ScenarioListRequest
{
  /// <summary>
  /// <see cref="Datasouce"/>
  /// </summary>
  public Datasource Datasouce { get; set; } = Datasource.Database;
}
