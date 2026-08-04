using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.Contracts.Scenarios;

/// <summary>
/// Požadavek na výpis scénářů
/// </summary>
public class ScenarioListRequest
{
  /// <summary>
  /// <see cref="Datasource"/>
  /// </summary>
  public Datasource Datasource { get; set; } = Datasource.Database;
}
