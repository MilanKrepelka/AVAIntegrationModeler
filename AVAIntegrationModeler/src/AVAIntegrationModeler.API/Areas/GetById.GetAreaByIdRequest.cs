using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.API.Areas;

/// <summary>
/// Požadavek na načtení oblasti podle identifikátoru.
/// </summary>
public class GetAreaByIdRequest
{
  public const string Route = "/Areas/by-id/{Datasource}/{AreaId:guid}";

  public static string BuildRoute(Guid areaId, Datasource datasource) =>
    Route
      .Replace("{Datasource}", datasource.ToString())
      .Replace("{AreaId:guid}", areaId.ToString());

  public static string BuildRoute(Guid areaId) =>
    BuildRoute(areaId, Datasource.Database);

  public Guid AreaId { get; set; }

  /// <summary>
  /// <see cref="AVAIntegrationModeler.Contracts.Datasource"/>
  /// </summary>
  public Datasource Datasource { get; set; } = Datasource.Database;
}
