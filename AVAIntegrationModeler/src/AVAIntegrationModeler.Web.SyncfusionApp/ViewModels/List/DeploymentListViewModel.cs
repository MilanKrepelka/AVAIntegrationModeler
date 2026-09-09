namespace AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;

/// <summary>
/// ViewModel pro zobrazení nasazení v seznamu.
/// </summary>
public class DeploymentListViewModel
{
  /// <summary>
  /// Jedinečný identifikátor nasazení.
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Kód nasazení.
  /// </summary>
  public string Code { get; init; } = string.Empty;

  /// <summary>
  /// Název nasazení.
  /// </summary>
  public string Name { get; init; } = string.Empty;

  /// <summary>
  /// Počet datových modelů v nasazení.
  /// </summary>
  public int DataModelCount { get; init; }

  /// <summary>
  /// Datum a čas posledního uložení nasazení (UTC).
  /// </summary>
  public DateTime? LastSaveDateTime { get; init; }

  /// <summary>
  /// Datum a čas posledního nasazení do AVAPlace (UTC).
  /// </summary>
  public DateTime? LastDeploymentDateTime { get; init; }

  /// <summary>
  /// Textová reprezentace ID pro vyhledávání v gridu.
  /// </summary>
  public string IdText => Id.ToString();
}
