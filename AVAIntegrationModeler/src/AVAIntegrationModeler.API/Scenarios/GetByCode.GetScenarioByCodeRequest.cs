using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.API.Scenarios;

public class GetScenarioByCodeRequest
{
  // Route now includes Datasource as enum text (no int constraint) and ScenarioCode as guid
  public const string Route = "/Scenarios/{" + nameof(Datasource) + "}/{" + nameof(ScenarioCode)+"}";
  // GuidRoute provided for compatibility/explicit naming, mirrors Route
  public const string GuidRoute = Route;

  public static string BuildRoute(string scenarioCode, Datasource datasource) =>
    Route
      .Replace("{" + nameof(Datasource) + "}", datasource.ToString())
      .Replace("{" + nameof(ScenarioCode) +"}", scenarioCode);

  // Backward-compatible helper if only scenarioCode is provided; defaults Datasource to Database
  public static string BuildRoute(string scenarioCode) =>
    BuildRoute(scenarioCode, Datasource.Database);

  public string ScenarioCode { get; set; } = string.Empty;

  /// <summary>
  /// <see cref="AVAIntegrationModeler.Contracts.Datasource"/>
  /// </summary>
  public Datasource Datasource { get; set; } = Datasource.Database;
}
