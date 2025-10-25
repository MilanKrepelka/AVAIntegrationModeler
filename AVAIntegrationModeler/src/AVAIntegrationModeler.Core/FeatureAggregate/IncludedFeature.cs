using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Domain.FeatureAggregate;

/// <summary>
/// Reprezentuje zahrnutou (vnořenou) featureId v rámci většího systému.
/// </summary>
/// <remarks>Třída slouží jako zástupce nebo označení pro featureId, které jsou součástí většího systému. Sama o sobě neobsahuje žádnou funkcionalitu,
/// ale může být použita k identifikaci nebo kategorizaci zahrnutých featureId v aplikaci.</remarks>
public class IncludedFeature
{
  /// <summary>
  /// Základní konstruktor.
  /// </summary>
  /// <param name="featureId">Instanční objekt <see cref="FeatureId"/> představující zahrnutou featureId.</param>
  /// <param name="consumeOnly">Indikuje, zda je featureId pouze pro konzumaci.</param>
  public IncludedFeature(Guid featureId, bool consumeOnly)
  {
    this.FeatureId = featureId;
    this.ConsumeOnly = consumeOnly;
  }

  /// <summary>
  /// Propojená featureId (<see cref="FeatureId"/>).
  /// </summary>
  public Guid FeatureId { get; init; }

  /// <summary>
  /// Příznak určující, zda je featureId pouze pro konzumaci.
  /// </summary>
  public bool ConsumeOnly { get; init; }
}
