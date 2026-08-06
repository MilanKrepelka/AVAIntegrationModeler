using System;

namespace AVAIntegrationModeler.Contracts.DTO;

/// <summary>
/// Výsledek importu jednoho datového modelu z AVAPlace do lokální databáze.
/// </summary>
public class ImportDataModelResultDTO
{
  /// <summary>
  /// Identifikátor datového modelu v AVAPlace.
  /// </summary>
  public Guid AvaPlaceModelId { get; set; }

  /// <summary>
  /// Kód datového modelu.
  /// </summary>
  public string Code { get; set; } = string.Empty;

  /// <summary>
  /// Příznak, zda import tohoto modelu proběhl úspěšně.
  /// </summary>
  public bool Success { get; set; }

  /// <summary>
  /// Popis chyby, pokud import tohoto modelu selhal.
  /// </summary>
  public string? ErrorMessage { get; set; }

  /// <summary>
  /// Identifikátor lokálně vytvořeného/aktualizovaného datového modelu, pokud byl import úspěšný.
  /// </summary>
  public Guid? LocalDataModelId { get; set; }
}
