using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVAIntegrationModeler.Core.ContributorAggregate;
using AVAIntegrationModeler.Core.ValueObjects;

namespace AVAIntegrationModeler.Core.ScenarioAggregate;

/// <summary>
/// Doménový objekt pro integrační scénář.
/// </summary>
public class Scenario : EntityBase, IAggregateRoot
{
  public Scenario(string code)
  {
    UpdateCode(code); // TODO: Replace with value object and use primary constructor to populate field.
  }
  
  /// <summary>
  /// Gets the unique code associated with this instance.
  /// </summary>
  public string Code { get; private set; } = default!;

  /// <summary>
  /// Název integračního scénáře.
  /// </summary>
  public LocalizedValue Name { get; private set; } = new LocalizedValue();

  /// <summary>
  /// Popis integračního scénáře.
  /// </summary>
  public LocalizedValue Decsription { get; private set; } = new LocalizedValue();

  /// <summary>
  /// Vstuppní feature integračního scénáře.
  /// </summary>
  public Feature? InputFeature { get; private set; }

  /// <summary>
  /// Výstupní feature integračního scénáře.
  /// </summary>
  public Feature? OutputFeature { get; private set; }

  public Scenario UpdateCode(string newCode)
  {
    Code = Guard.Against.NullOrEmpty(newCode, nameof(newCode));
    return this;
  }
}
