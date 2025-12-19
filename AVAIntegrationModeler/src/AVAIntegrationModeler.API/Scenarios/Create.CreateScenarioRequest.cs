using System.ComponentModel.DataAnnotations;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.API.Scenarios;

public class CreateScenarioRequest
{
  public const string Route = "/Scenarios";

  [Required]
  public Datasource Datasource { get; set; } = Datasource.Database;

  [Required]
  public ScenarioDTO Scenario { get; set; } = new ScenarioDTO();
}
