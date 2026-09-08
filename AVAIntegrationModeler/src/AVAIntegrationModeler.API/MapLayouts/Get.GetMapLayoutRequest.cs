namespace AVAIntegrationModeler.API.MapLayouts;

/// <summary>
/// Požadavek na načtení MapLayout podle klíče.
/// </summary>
public class GetMapLayoutRequest
{
  public const string Route = "/MapLayouts/{Key}";

  public static string BuildRoute(string key) =>
    Route.Replace("{Key}", Uri.EscapeDataString(key));

  public string Key { get; set; } = string.Empty;
}
