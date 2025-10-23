using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Contracts.DTO;
// TODO Začni rovnou s agregátem Feature, přidej potřebné vlastnosti podle doménového modelu Feature.

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

  /// <summary>
  /// Seznam featur zahrnutých v této integrační feature.
  /// </summary>
  public List<(Guid FeatureId, bool ConsumeOnly)> IncludedFeatures { get; init; } = new();

  /// <summary>
  /// Seznam modelů zahrnutých v této integrační feature.
  /// </summary>
  public List<Guid> IncludedModels { get; init; } = new();
}

