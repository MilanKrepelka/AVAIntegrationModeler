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

  /// <summary>
  /// Datové modely patřící do této oblasti.
  /// </summary>
  public List<DataModelListViewModel> DataModels { get; set; } = [];

  /// <summary>
  /// Textová reprezentace kódů datových modelů.
  /// </summary>
  public string DataModelsText => DataModels.Count > 0
    ? string.Join(", ", DataModels.Select(m => m.Code))
    : string.Empty;
}
