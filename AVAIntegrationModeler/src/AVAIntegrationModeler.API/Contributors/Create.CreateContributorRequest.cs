using System.ComponentModel.DataAnnotations;

namespace AVAIntegrationModeler.API.Contributors;

public class CreateContributorRequest
{
  public const string Route = "/Scenarios";

  [Required]
  public string? Name { get; set; }
  public string? PhoneNumber { get; set; }
}
