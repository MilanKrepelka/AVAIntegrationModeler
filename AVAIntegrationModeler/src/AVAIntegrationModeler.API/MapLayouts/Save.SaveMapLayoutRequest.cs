using System.ComponentModel.DataAnnotations;

namespace AVAIntegrationModeler.API.MapLayouts;

/// <summary>
/// Požadavek na uložení JSON diagramu MapLayout.
/// </summary>
public class SaveMapLayoutRequest
{
  public const string Route = "/MapLayouts/{Key}";

  public static string BuildRoute(string key) =>
    Route.Replace("{Key}", Uri.EscapeDataString(key));

  [Required] public string Key { get; set; } = string.Empty;
  [Required] public string DiagramJson { get; set; } = string.Empty;
}
