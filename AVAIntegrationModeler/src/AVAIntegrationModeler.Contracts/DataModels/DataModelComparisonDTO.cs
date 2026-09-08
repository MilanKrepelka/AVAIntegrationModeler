namespace AVAIntegrationModeler.Contracts.DataModels;

/// <summary>
/// Výsledek hloubkového porovnání datového modelu (včetně polí) mezi databází a AVAPlace.
/// </summary>
public record DataModelComparisonDTO
{
  /// <summary>Identifikátor datového modelu.</summary>
  public Guid DataModelId { get; init; }
  /// <summary>Kód datového modelu (z dostupného zdroje).</summary>
  public string Code { get; init; } = string.Empty;
  /// <summary>Název datového modelu (z dostupného zdroje).</summary>
  public string Name { get; init; } = string.Empty;
  /// <summary>Existuje model v lokální databázi?</summary>
  public bool ExistsInDatabase { get; init; }
  /// <summary>Existuje model v AVAPlace?</summary>
  public bool ExistsInAvaPlace { get; init; }
  /// <summary>Souhrnný stav modelu (bez polí).</summary>
  public ComparisonStatus ModelStatus { get; init; }
  /// <summary>Existuje alespoň jeden rozdíl (v modelu nebo v polích)?</summary>
  public bool HasDifferences { get; init; }
  /// <summary>Rozdíly na úrovni vlastností modelu.</summary>
  public List<DataModelPropertyDiffDTO> PropertyDiffs { get; init; } = new();
  /// <summary>Porovnání jednotlivých polí datového modelu.</summary>
  public List<DataModelFieldComparisonDTO> FieldComparisons { get; init; } = new();
}
