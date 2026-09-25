namespace AVAIntegrationModeler.Contracts.DTO;

/// <summary>
/// Výsledek importu záznamů datového modelu z XLS souboru.
/// </summary>
/// <param name="CreatedCount">Počet nově vytvořených záznamů.</param>
/// <param name="UpdatedCount">Počet aktualizovaných záznamů.</param>
/// <param name="Errors">Chyby na jednotlivých řádcích XLS souboru.</param>
public record ImportDataModelRecordsXlsResult(
  int CreatedCount,
  int UpdatedCount,
  IReadOnlyList<XlsImportRowError> Errors);

/// <summary>
/// Chyba při importu záznamu z konkrétního řádku XLS souboru.
/// </summary>
/// <param name="RowNumber">Číslo řádku v XLS souboru (řádek 1 = záhlaví, data začínají od řádku 2).</param>
/// <param name="Message">Popis chyby.</param>
public record XlsImportRowError(int RowNumber, string Message);
