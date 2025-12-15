using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.API.Scenarios;

public class GetScenarioByIdRequest
{
  // Route now includes Datasource as enum text (no int constraint) and ScenarioId as guid
  public const string Route = "/Scenarios/{" + nameof(Datasource) + "}/{" + nameof(ScenarioId) + ":guid}";
  // GuidRoute provided for compatibility/explicit naming, mirrors Route
  public const string GuidRoute = Route;

  public static string BuildRoute(Guid scenarioId, Datasource datasource) =>
    Route
      .Replace("{" + nameof(Datasource) + "}", datasource.ToString())
      .Replace("{" + nameof(ScenarioId) + ":guid}", scenarioId.ToString());

  // Backward-compatible helper if only scenarioId is provided; defaults Datasource to Database
  public static string BuildRoute(Guid scenarioId) =>
    BuildRoute(scenarioId, Datasource.Database);

  public Guid ScenarioId { get; set; }

  /// <summary>
  /// <see cref="AVAIntegrationModeler.Contracts.Datasource"/>
  /// </summary>
  public Datasource Datasource { get; set; } = Datasource.Database;
}
