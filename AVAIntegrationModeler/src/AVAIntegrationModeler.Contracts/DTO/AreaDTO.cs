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
}
