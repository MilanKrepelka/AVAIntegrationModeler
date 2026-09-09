using AVAIntegrationModeler.Contracts.DataModels;

namespace AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.DataModelChanges;

/// <summary>
/// ViewModel pro stránku porovnání datového modelu.
/// </summary>
public class DataModelChangesViewModel
{
  /// <summary>Výsledek porovnání načtený z API.</summary>
  public DataModelComparisonDTO? Comparison { get; set; }
  /// <summary>Zobrazit i shodná pole?</summary>
  public bool ShowSameFields { get; set; } = false;
  /// <summary>Filtrovaný seznam polí dle ShowSameFields.</summary>
  public IEnumerable<DataModelFieldComparisonDTO> VisibleFields =>
    Comparison?.FieldComparisons.Where(f => ShowSameFields || f.Status != ComparisonStatus.Same)
    ?? Enumerable.Empty<DataModelFieldComparisonDTO>();
}
