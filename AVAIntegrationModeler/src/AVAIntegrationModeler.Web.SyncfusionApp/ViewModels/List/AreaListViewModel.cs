namespace AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;

/// <summary>
/// ViewModel pro zobrazení oblasti v seznamu.
/// </summary>
public class AreaListViewModel
{
  /// <summary>
  /// Jedinečný identifikátor oblasti.
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Kód oblasti.
  /// </summary>
  public string Code { get; init; } = string.Empty;

  /// <summary>
  /// Název oblasti.
  /// </summary>
  public string Name { get; init; } = string.Empty;
}
