using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Contracts.DTO;
/// <summary>
/// DTO pro integrační scénář.
/// </summary>
public record FeatureDTO
{
  /// <summary>
  /// Jedinečný identifikátor scénáře.
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Kód scénáře, který slouží k jeho jednoznačné identifikaci v rámci systému.
  /// </summary>
  public string Code { get; init; } = string.Empty;

  /// <summary>
  /// Lokalizovaný název scénáře (např. v češtině a angličtině).
  /// </summary>
  public LocalizedValue Name { get; init; } = new();

  /// <summary>
  /// Lokalizovaný popis scénáře (např. v češtině a angličtině).
  /// </summary>
  public LocalizedValue Description { get; init; } = new();
}

