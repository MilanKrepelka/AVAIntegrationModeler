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
public class Scenario : EntityBase<Guid>, IAggregateRoot
{
  /// <summary>
  /// Konstruktor pro vytvoření nového scénáře.
  /// </summary>
  /// <param name="id">Identifikátor scénáře.</param>
  public Scenario(Guid id)
  {
    SetId(id); 
  }
  
  /// <summary>
  /// Vrací jedinečný kód přiřazený této instanci.
  /// </summary>
  public string Code { get; private set; } = default!;

  /// <summary>
  /// Název integračního scénáře (lokalizovaný).
  /// </summary>
  public LocalizedValue Name { get; private set; } = new LocalizedValue();

  /// <summary>
  /// Popis integračního scénáře (lokalizovaný).
  /// </summary>
  public LocalizedValue Description { get; private set; } = new LocalizedValue();

  /// <summary>
  /// Vstupní feature integračního scénáře.
  /// </summary>
  public Feature? InputFeature { get; private set; }

  /// <summary>
  /// Výstupní feature integračního scénáře.
  /// </summary>
  public Feature? OutputFeature { get; private set; }

  /// <summary>
  /// Nastaví kód scénáře.
  /// </summary>
  /// <param name="newCode">Nový kód scénáře.</param>
  public Scenario SetCode(string newCode)
  {
    Code = Guard.Against.NullOrEmpty(newCode, nameof(newCode));
    return this;
  }

  /// <summary>
  /// Aktualizuje identifikátor scénáře.
  /// </summary>
  /// <param name="newCode">Nový identifikátor.</param>
  public Scenario SetId(Guid newId)
  {
    Id = newId;
    return this;
  }

  /// <summary>
  /// Nastaví název scénáře.
  /// </summary>
  /// <param name="name">Nový lokalizovaný název.</param>
  public Scenario SetName(LocalizedValue name)
  {
    Name = name;
    return this;
  }

  /// <summary>
  /// Nastaví popis scénáře.
  /// </summary>
  /// <param name="description">Nový lokalizovaný popis.</param>
  public Scenario SetDescription(LocalizedValue description)
  {
    Description = description;
    return this;
  }

  /// <summary>
  /// Nastaví vstupní feature scénáře.
  /// </summary>
  /// <param name="feature">Vstupní feature.</param>
  public Scenario SetInputFeature(Feature? feature)
  {
    InputFeature = feature;
    return this;
  }

  /// <summary>
  /// Nastaví výstupní feature scénáře.
  /// </summary>
  /// <param name="feature">Výstupní feature.</param>
  public Scenario SetOutputFeature(Feature? feature)
  {
    OutputFeature = feature;
    return this;
  }
}
