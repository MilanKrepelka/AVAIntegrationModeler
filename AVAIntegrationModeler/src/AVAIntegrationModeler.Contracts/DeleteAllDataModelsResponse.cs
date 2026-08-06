namespace AVAIntegrationModeler.Contracts;

/// <summary>
/// Odpověď obsahující souhrn smazání všech datových modelů a jejich záznamů z databáze.
/// </summary>
public class DeleteAllDataModelsResponse
{
  /// <summary>
  /// Počet smazaných datových modelů.
  /// </summary>
  public int DeletedModelsCount { get; set; }

  /// <summary>
  /// Počet smazaných záznamů datových modelů (DataModelRecord).
  /// </summary>
  public int DeletedRecordsCount { get; set; }
}
