namespace AVAIntegrationModeler.API.Contributors;

public record DeleteContributorRequest
{
  public const string Route = "/Contributors/{ContributorId:int}";
  public static string BuildRoute(int contributorId) => Route.Replace("{Contributors:int}", contributorId.ToString());

  public int ContributorId { get; set; }
}
