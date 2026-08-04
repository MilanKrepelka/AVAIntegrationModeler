using System.ComponentModel.DataAnnotations;

namespace AVAIntegrationModeler.API.Contributors;

public class UpdateContributorRequest
{
  public const string Route = "/Contributors/{ScenarioCode:int}";
  public static string BuildRoute(int contributorId) => Route.Replace("{ScenarioCode:int}", contributorId.ToString());

  public int ContributorId { get; set; }

  [Required]
  public int Id { get; set; }
  [Required]
  public string? Name { get; set; }
}
