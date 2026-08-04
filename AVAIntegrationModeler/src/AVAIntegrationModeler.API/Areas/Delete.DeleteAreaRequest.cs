using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.API.Areas;

/// <summary>
/// Požadavek na smazání oblasti.
/// </summary>
public record DeleteAreaRequest
{
  public const string Route = "/Areas/{Datasource}/{AreaCode}";

  public static string BuildRoute(Datasource datasource, string areaCode) =>
    Route
      .Replace("{Datasource}", datasource.ToString())
      .Replace("{AreaCode}", Uri.EscapeDataString(areaCode));

  /// <summary>
  /// Kód oblasti, která má být smazána.
  /// </summary>
  public string AreaCode { get; set; } = string.Empty;

  /// <summary>
  /// Datový zdroj, ze kterého má být oblast smazána.
  /// </summary>
  public Datasource Datasource { get; set; }
}
