namespace AVAIntegrationModeler.API.Contributors;

public class GetContributorByIdRequest
{
  public const string Route = "/Contributors/{ScenarioId:int}";
  public static string BuildRoute(int contributorId) => Route.Replace("{ScenarioId:int}", contributorId.ToString());

  public int ContributorId { get; set; }
}
