using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.API.Areas;

/// <summary>
/// Požadavek na načtení oblasti podle kódu.
/// </summary>
public class GetAreaByCodeRequest
{
  public const string Route = "/Areas/{Datasource}/{AreaCode}";

  public static string BuildRoute(Datasource datasource, string areaCode) =>
    Route
      .Replace("{Datasource}", datasource.ToString())
      .Replace("{AreaCode}", Uri.EscapeDataString(areaCode));

  public Datasource Datasource { get; set; } = Datasource.Database;
  public string AreaCode { get; set; } = string.Empty;
}
