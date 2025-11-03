using System.ComponentModel.DataAnnotations;

namespace AVAIntegrationModeler.API.Contributors;

public class CreateContributorRequest
{
  public const string Route = "/DataModels";

  [Required]
  public string? Name { get; set; }
  public string? PhoneNumber { get; set; }
}
