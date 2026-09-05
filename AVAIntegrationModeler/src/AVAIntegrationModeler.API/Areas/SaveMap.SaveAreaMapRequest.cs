using System.ComponentModel.DataAnnotations;

namespace AVAIntegrationModeler.API.Areas;

/// <summary>
/// Požadavek na uložení JSON diagramu mapy oblasti.
/// </summary>
public class SaveAreaMapRequest
{
  public const string Route = "/Areas/{AreaId}/map";

  public static string BuildRoute(Guid areaId) =>
    Route.Replace("{AreaId}", areaId.ToString());

  /// <summary>ID oblasti.</summary>
  [Required]
  public Guid AreaId { get; set; }

  /// <summary>JSON stav diagramu ze SfDiagramComponent.SaveDiagram().</summary>
  [Required]
  public string DiagramJson { get; set; } = string.Empty;
}
