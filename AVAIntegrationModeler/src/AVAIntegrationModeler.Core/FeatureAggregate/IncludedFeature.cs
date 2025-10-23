using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Core.FeatureAggregate;

/// <summary>
/// Reprezentuje zahrnutou (vnořenou) feature v rámci většího systému.
/// </summary>
/// <remarks>Třída slouží jako zástupce nebo označení pro feature, které jsou součástí většího systému. Sama o sobě neobsahuje žádnou funkcionalitu,
/// ale může být použita k identifikaci nebo kategorizaci zahrnutých feature v aplikaci.</remarks>
public class IncludedFeature
{
  /// <summary>
  /// Základní konstruktor.
  /// </summary>
  /// <param name="feature">Instanční objekt <see cref="Feature"/> představující zahrnutou feature.</param>
  /// <param name="consumeOnly">Indikuje, zda je feature pouze pro konzumaci.</param>
  public IncludedFeature(Feature feature, bool consumeOnly)
  {
    this.Feature = feature;
    this.ConsumeOnly = consumeOnly;
  }

  /// <summary>
  /// Propojená feature (<see cref="Feature"/>).
  /// </summary>
  public Feature Feature { get; init; }

  /// <summary>
  /// Příznak určující, zda je feature pouze pro konzumaci.
  /// </summary>
  public bool ConsumeOnly { get; init; }
}
