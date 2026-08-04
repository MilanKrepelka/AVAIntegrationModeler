using System.ComponentModel.DataAnnotations;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.API.Areas;

/// <summary>
/// Požadavek na aktualizaci oblasti.
/// </summary>
public class UpdateAreaRequest
{
  public const string Route = "/Areas/{Datasource}/{AreaCode}";

  public static string BuildRoute(string areaCode, Datasource datasource) =>
    Route
      .Replace("{Datasource}", datasource.ToString())
      .Replace("{AreaCode}", areaCode);

  public static string BuildRoute(string areaCode) =>
    BuildRoute(areaCode, Datasource.Database);

  [Required]
  public string AreaCode { get; set; } = string.Empty;

  [Required]
  public Datasource Datasource { get; set; } = Datasource.Database;

  [Required]
  public AreaDTO Area { get; set; } = new AreaDTO();
}
