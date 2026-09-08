namespace AVAIntegrationModeler.Contracts.DataModels;

/// <summary>
/// Souhrnné porovnání všech DataModelů nasazení mezi databází a AVAPlace.
/// </summary>
public record DeploymentChangesSummaryDTO
{
  /// <summary>Kód nasazení.</summary>
  public string DeploymentCode { get; init; } = string.Empty;
  /// <summary>Název nasazení.</summary>
  public string DeploymentName { get; init; } = string.Empty;
  /// <summary>Celkový počet DataModelů v nasazení.</summary>
  public int TotalModels { get; init; }
  /// <summary>Počet modelů shodných v obou zdrojích.</summary>
  public int ModelsSame { get; init; }
  /// <summary>Počet modelů s rozdíly.</summary>
  public int ModelsDifferent { get; init; }
  /// <summary>Počet modelů pouze v lokální databázi.</summary>
  public int ModelsOnlyInDatabase { get; init; }
  /// <summary>Počet modelů pouze v AVAPlace.</summary>
  public int ModelsOnlyInAvaPlace { get; init; }
  /// <summary>Existuje alespoň jeden rozdíl napříč všemi modely?</summary>
  public bool HasDifferences { get; init; }
  /// <summary>Souhrnné položky za jednotlivé modely.</summary>
  public List<DataModelComparisonSummaryItemDTO> Models { get; init; } = new();
}
