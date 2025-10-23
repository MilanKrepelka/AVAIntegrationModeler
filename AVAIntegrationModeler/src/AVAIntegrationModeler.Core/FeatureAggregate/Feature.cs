using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVAIntegrationModeler.Core.ValueObjects;

namespace AVAIntegrationModeler.Core.FeatureAggregate;

/// <summary>
/// Integrační feature v agregátu scénáře.
/// </summary>
public class Feature : EntityBase<Guid>, IAggregateRoot
{
  /// <summary>
  /// Základní konstruktor pro vytvoření nové integrační feature.
  /// </summary>
  /// <param name="id"></param>
  public Feature(Guid id)
  {
    this.Id = id;
  }

  /// <summary>
  /// Techický kód feature, který slouží k jeho jednoznačné identifikaci v rámci systému.
  /// </summary>
  public string Code { get; internal set; } = string.Empty;

  /// <summary>
  /// Název integrační feature (lokalizovaný).
  /// </summary>
  public LocalizedValue Name { get; internal set; } = new();

  /// <summary>
  /// Popis integrační feature (lokalizovaný).
  /// </summary>
  public LocalizedValue Description { get; internal set; } = new();

  /// <summary>
  /// Feature zahrnuté v této integrační feature. Featury mohou být použity jako součást této feature.
  /// </summary>
  public List<IncludedFeature> IncludedFeatures { get; internal set; } = new();

  /// <summary>
  /// Modely zahrnuté v této integrační feature. 
  /// </summary>
  public List<IncludedModel> IncludedModels{ get; internal set; } = new();

}
