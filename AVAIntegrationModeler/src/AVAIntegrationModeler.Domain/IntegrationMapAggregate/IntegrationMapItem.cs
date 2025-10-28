using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;

namespace AVAIntegrationModeler.Domain.IntegrationMapAggregate;

/// <summary>
/// Položka integrační mapy - reprezentuje jeden scénář a jeho aktivační klíče.
/// </summary>
public class IntegrationMapItem : EntityBase<Guid>
{
  /// <summary>
  /// Privátní kolekce aktivačních klíčů.
  /// </summary>
  private readonly List<ActivationKey> _keys = new();

  /// <summary>
  /// Konstruktor pro vytvoření nové integrační položky.
  /// </summary>
  /// <param name="id">Identifikátor položky.</param>
  /// <param name="scenarioId">Identifikátor scénáře.</param>
  public IntegrationMapItem(Guid id, Guid scenarioId)
  {
    Id = Guard.Against.Default(id, nameof(id));
    ScenarioId = Guard.Against.Default(scenarioId, nameof(scenarioId));
  }

  /// <summary>
  /// Privátní konstruktor pro EF Core.
  /// </summary>
  private IntegrationMapItem()
  {
    // EF Core vyžaduje bezparametrový konstruktor
  }

  /// <summary>
  /// Identifikátor scénáře spojeného s touto položkou.
  /// </summary>
  public Guid ScenarioId { get; private set; }

  /// <summary>
  /// Veřejný read-only přístup k aktivačním klíčům.
  /// </summary>
  public IReadOnlyCollection<ActivationKey> Keys => _keys.AsReadOnly();

  /// <summary>
  /// Přidá nový aktivační klíč.
  /// </summary>
  /// <param name="key">Hodnota aktivačního klíče.</param>
  /// <returns>Nově vytvořený aktivační klíč.</returns>
  public ActivationKey AddKey(string key)
  {
    Guard.Against.NullOrWhiteSpace(key, nameof(key));

    // Business rule: nelze přidat duplicitní klíč (case-insensitive)
    if (_keys.Any(k => k.Key.Equals(key, StringComparison.OrdinalIgnoreCase)))
    {
      throw new InvalidOperationException($"Activation key '{key}' already exists.");
    }

    var activationKey = new ActivationKey(Guid.NewGuid(), key);
    _keys.Add(activationKey);

    return activationKey;
  }

  /// <summary>
  /// Odebere aktivační klíč podle jeho hodnoty.
  /// </summary>
  /// <param name="key">Hodnota klíče k odebrání.</param>
  /// <returns>True, pokud byl klíč odebrán, jinak false.</returns>
  public bool RemoveKey(string key)
  {
    var activationKey = _keys.FirstOrDefault(k => 
      k.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
    
    if (activationKey == null)
    {
      return false;
    }

    return _keys.Remove(activationKey);
  }

  /// <summary>
  /// Odebere aktivační klíč podle jeho ID.
  /// </summary>
  /// <param name="keyId">ID klíče k odebrání.</param>
  /// <returns>True, pokud byl klíč odebrán, jinak false.</returns>
  public bool RemoveKeyById(Guid keyId)
  {
    var activationKey = _keys.FirstOrDefault(k => k.Id == keyId);
    
    if (activationKey == null)
    {
      return false;
    }

    return _keys.Remove(activationKey);
  }

  /// <summary>
  /// Získá aktivační klíč podle jeho hodnoty.
  /// </summary>
  /// <param name="key">Hodnota klíče.</param>
  /// <returns>Klíč nebo null, pokud není nalezen.</returns>
  public ActivationKey? GetKey(string key)
  {
    return _keys.FirstOrDefault(k => 
      k.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
  }

  /// <summary>
  /// Vymaže všechny aktivační klíče.
  /// </summary>
  public void ClearKeys()
  {
    _keys.Clear();
  }
}
