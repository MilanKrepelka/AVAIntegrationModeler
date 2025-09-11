namespace AVAIntegrationModeler.API.Scenarios;

/// <summary>
/// Požadavek na smazání scénáře podle jeho ID.
/// </summary>
public record DeleteScenarioRequest
{
  public const string Route = "/Scenarios/{ScenarioId:guid}";
  public static string BuildRoute(int scenarioId) => Route.Replace("{ScenarioId:guid}", scenarioId.ToString());
  public Guid ScenarioId { get; set; }
}
