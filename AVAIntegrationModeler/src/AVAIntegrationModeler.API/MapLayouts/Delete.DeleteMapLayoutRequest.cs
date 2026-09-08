namespace AVAIntegrationModeler.API.MapLayouts;

/// <summary>
/// Požadavek na smazání MapLayout podle klíče.
/// </summary>
public class DeleteMapLayoutRequest
{
  public const string Route = "/MapLayouts/{Key}";

  public static string BuildRoute(string key) =>
    Route.Replace("{Key}", Uri.EscapeDataString(key));

  public string Key { get; set; } = string.Empty;
}
