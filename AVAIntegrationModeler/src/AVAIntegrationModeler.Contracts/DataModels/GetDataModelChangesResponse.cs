namespace AVAIntegrationModeler.Contracts.DataModels;

/// <summary>
/// Odpověď obsahující výsledek porovnání datového modelu.
/// </summary>
public class GetDataModelChangesResponse
{
  /// <summary>Výsledek hloubkového porovnání.</summary>
  public DataModelComparisonDTO? Comparison { get; set; }
}
