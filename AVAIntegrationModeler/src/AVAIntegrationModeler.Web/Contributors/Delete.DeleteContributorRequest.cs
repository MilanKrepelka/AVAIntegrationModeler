namespace AVAIntegrationModeler.API.Contributors;

public record DeleteContributorRequest
{
  public const string Route = "/Scenarios/{ScenarioId:int}";
  public static string BuildRoute(int contributorId) => Route.Replace("{ScenarioId:int}", contributorId.ToString());

  public int ContributorId { get; set; }
}
