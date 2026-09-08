using System;

namespace AVAIntegrationModeler.Contracts.DTO;

/// <summary>
/// DTO pro oblast (Area).
/// </summary>
public record AreaDTO
{
  /// <summary>
  /// Jedinečný identifikátor oblasti.
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Kód oblasti, který slouží k její jednoznačné identifikaci v rámci systému.
  /// </summary>
  public string Code { get; init; } = string.Empty;

  /// <summary>
  /// Název oblasti.
  /// </summary>
  public string Name { get; init; } = string.Empty;

  /// <summary>
  /// Datum a čas vytvoření oblasti v databázi (UTC).
  /// </summary>
  public DateTime? CreatedAt { get; init; }

  /// <summary>
  /// Datum a čas posledního uložení oblasti do databáze (UTC).
  /// </summary>
  public DateTime? LastSavedAt { get; init; }

  /// <summary>
  /// Datum a čas posledního uložení diagramu mapy oblasti (UTC).
  /// </summary>
  public DateTime? LastMapSave { get; init; }

  /// <summary>
  /// JSON reprezentace uloženého diagramu mapy oblasti.
  /// </summary>
  public string? MapDiagramJson { get; init; }
}
