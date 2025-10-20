using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.API.Scenarios;

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
