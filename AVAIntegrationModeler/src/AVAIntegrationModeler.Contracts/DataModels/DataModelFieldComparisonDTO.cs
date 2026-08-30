using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Contracts.DataModels;

/// <summary>
/// Výsledek porovnání jednoho pole datového modelu mezi databází a AVAPlace.
/// </summary>
public record DataModelFieldComparisonDTO
{
  /// <summary>Identifikátor pole.</summary>
  public Guid FieldId { get; init; }
  /// <summary>Název pole (z dostupného zdroje).</summary>
  public string FieldName { get; init; } = string.Empty;
  /// <summary>Výsledný stav porovnání pole.</summary>
  public ComparisonStatus Status { get; init; }
  /// <summary>Data pole z databáze, nebo null pokud pole v databázi neexistuje.</summary>
  public DataModelFieldDTO? DatabaseField { get; init; }
  /// <summary>Data pole z AVAPlace, nebo null pokud pole v AVAPlace neexistuje.</summary>
  public DataModelFieldDTO? AvaPlaceField { get; init; }
  /// <summary>Seznam vlastností, ve kterých se pole liší (pouze při Status == Different).</summary>
  public List<DataModelPropertyDiffDTO> PropertyDiffs { get; init; } = new();
}
