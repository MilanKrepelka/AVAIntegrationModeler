namespace AVAIntegrationModeler.API.Scenarios;

public class GetScenarioByIdRequest
{
  public const string Route = "/Scenarios/{" + nameof(ScenarioId) + ":guid}";
  public static string BuildRoute(Guid scenarioId) => Route.Replace("{" + nameof(ScenarioId) + ":guid}", scenarioId.ToString());
  public Guid ScenarioId { get; set; }
}
