using System.Collections.Generic;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Contracts;

/// <summary>
/// Odpověď obsahující souhrn hromadného importu datových modelů z AVAPlace do lokální databáze.
/// </summary>
public class ImportAllDataModelsFromAvaPlaceResponse
{
  /// <summary>
  /// Výsledky importu jednotlivých datových modelů.
  /// </summary>
  public List<ImportDataModelResultDTO> Results { get; set; } = new();

  /// <summary>
  /// Počet úspěšně importovaných datových modelů.
  /// </summary>
  public int SuccessCount { get; set; }

  /// <summary>
  /// Počet datových modelů, jejichž import selhal.
  /// </summary>
  public int FailedCount { get; set; }
}
