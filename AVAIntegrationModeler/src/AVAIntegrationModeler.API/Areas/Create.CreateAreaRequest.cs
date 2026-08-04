using System.ComponentModel.DataAnnotations;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.API.Areas;

/// <summary>
/// Požadavek na vytvoření oblasti.
/// </summary>
public class CreateAreaRequest
{
  public const string Route = "/Areas";

  [Required]
  public Datasource Datasource { get; set; } = Datasource.Database;

  [Required]
  public AreaDTO Area { get; set; } = new AreaDTO();
}
