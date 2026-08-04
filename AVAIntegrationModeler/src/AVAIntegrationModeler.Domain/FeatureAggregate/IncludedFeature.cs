using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Domain.FeatureAggregate;

/// <summary>
/// Reprezentuje zahrnutou (vnořenou) feature v rámci většího systému.
/// </summary>
/// <remarks>Třída slouží jako zástupce nebo označení pro feature, které jsou součástí většího systému. Sama o sobě neobsahuje žádnou funkcionalitu,
/// ale může být použita k identifikaci nebo kategorizaci zahrnutých features v aplikaci.</remarks>
public class IncludedFeature : EntityBase<Guid>
{
  /// <summary>
  /// Základní konstruktor.
  /// </summary>
  /// <param name="featureId">Instanční objekt <see cref="Feature"/> představující zahrnutou feature.</param>
  /// <param name="consumeOnly">Indikuje, zda je feature pouze pro konzumaci.</param>
  public IncludedFeature(Guid featureId, bool consumeOnly)
  {
    Id = Guid.NewGuid(); // Generování primárního klíče
    FeatureId = featureId;
    ConsumeOnly = consumeOnly;
  }

  /// <summary>
  /// Privátní konstruktor pro EF Core.
  /// </summary>
  private IncludedFeature()
  {
    // EF Core vyžaduje bezparametrový konstruktor
  }

  /// <summary>
  /// Propojená feature (<see cref="Feature"/>).
  /// </summary>
  public Guid FeatureId { get; private set; }

  /// <summary>
  /// Příznak určující, zda je feature pouze pro konzumaci.
  /// </summary>
  public bool ConsumeOnly { get; private set; }
}
