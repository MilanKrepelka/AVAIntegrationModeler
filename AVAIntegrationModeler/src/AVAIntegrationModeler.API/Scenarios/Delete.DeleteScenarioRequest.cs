using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.API.Scenarios;

/// <summary>
/// Požadavek na smazání scénáře podle jeho kódu a datového zdroje.
/// </summary>
public record DeleteScenarioRequest
{
  // ✅ Doporučený formát: /Scenarios/{Datasource}/{ScenarioCode}
  public const string Route = "/Scenarios/{Datasource}/{ScenarioCode}";
  
  public static string BuildRoute(Datasource datasource, string scenarioCode) => 
    Route
      .Replace("{Datasource}", datasource.ToString())
      .Replace("{ScenarioCode}", Uri.EscapeDataString(scenarioCode));
  
  /// <summary>
  /// Kód scénáře, který má být smazán.
  /// </summary>
  public string ScenarioCode { get; set; } = string.Empty;

  /// <summary>
  /// Datový zdroj, ze kterého má být scénář smazán.
  /// </summary>
  public Datasource Datasource { get; set; }
}
