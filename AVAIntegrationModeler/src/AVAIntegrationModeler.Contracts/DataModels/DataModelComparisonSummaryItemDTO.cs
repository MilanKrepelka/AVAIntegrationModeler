namespace AVAIntegrationModeler.Contracts.DataModels;

/// <summary>
/// Souhrnná položka porovnání jednoho DataModelu v rámci přehledu nasazení.
/// </summary>
public record DataModelComparisonSummaryItemDTO
{
  /// <summary>Identifikátor DataModelu.</summary>
  public Guid DataModelId { get; init; }
  /// <summary>Kód DataModelu.</summary>
  public string Code { get; init; } = string.Empty;
  /// <summary>Název DataModelu.</summary>
  public string Name { get; init; } = string.Empty;
  /// <summary>Výsledný stav porovnání modelu.</summary>
  public ComparisonStatus Status { get; init; }
  /// <summary>Existuje alespoň jeden rozdíl (vlastnost modelu nebo pole)?</summary>
  public bool HasDifferences { get; init; }
  /// <summary>Počet rozdílných vlastností modelu.</summary>
  public int PropertyDiffCount { get; init; }
  /// <summary>Počet polí lišících se mezi DB a AVAPlace.</summary>
  public int FieldsDifferent { get; init; }
  /// <summary>Počet polí pouze v lokální databázi.</summary>
  public int FieldsOnlyInDatabase { get; init; }
  /// <summary>Počet polí pouze v AVAPlace.</summary>
  public int FieldsOnlyInAvaPlace { get; init; }
}
