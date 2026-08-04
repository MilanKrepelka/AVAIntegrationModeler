using System.ComponentModel.DataAnnotations;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.API.Scenarios;

/// <summary>
/// Command pro aktualizaci scénáře podle jeho kódu nebo Code.
/// </summary>
public class UpdateScenarioRequest
{
  // PUT /Scenarios/{Datasource}/{ScenarioCode}
  public const string Route = "/Scenarios/{Datasource}/{ScenarioCode}";

  public static string BuildRoute(string scenarioCode, Datasource datasource) =>
    Route
      .Replace("{Datasource}", datasource.ToString())
      .Replace("{ScenarioCode}", scenarioCode);

  public static string BuildRoute(string scenarioCode) =>
    BuildRoute(scenarioCode, Datasource.Database);

  [Required]
  public string ScenarioCode { get; set; } = string.Empty;

  [Required]
  public Datasource Datasource { get; set; } = Datasource.Database;

  [Required]
  public ScenarioDTO Scenario { get; set; } = new ScenarioDTO();
}
