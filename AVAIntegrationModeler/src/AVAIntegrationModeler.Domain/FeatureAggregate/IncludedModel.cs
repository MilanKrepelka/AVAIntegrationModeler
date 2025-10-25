using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Domain.FeatureAggregate;

/// <summary>
/// Model AVA virtuálního modelu
/// </summary>
public class IncludedModel
{
  /// <summary>
  /// Základní konstruktor
  /// </summary>
  /// <param name="modelId">Identifikátor modelu</param>
  public IncludedModel(Guid modelId)
  {
    ModelId = modelId;
  }

  /// <summary>
  /// Příznak určující, zda je model pouze pro konzumaci
  /// </summary>
  public bool ConsumeOnly { get; set; }

  /// <summary>
  /// Identifikátor modelu
  /// </summary>
  public Guid ModelId { get; }

}
