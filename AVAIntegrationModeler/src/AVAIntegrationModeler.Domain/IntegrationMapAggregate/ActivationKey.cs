using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;

namespace AVAIntegrationModeler.Domain.IntegrationMapAggregate;

/// <summary>
/// Aktivační klíč pro integrační scénář.
/// </summary>
/// <remarks>
/// Aktivační klíč určuje, za jakých podmínek (klíčů) se má daný integrační scénář spustit.
/// Například klíč může být "ORDER_CREATED", "CUSTOMER_UPDATED" apod.
/// </remarks>
public class ActivationKey : EntityBase<Guid>
{
  /// <summary>
  /// Konstruktor pro vytvoření nového aktivačního klíče.
  /// </summary>
  /// <param name="id">Identifikátor klíče.</param>
  /// <param name="key">Hodnota klíče.</param>
  public ActivationKey(Guid id, string key)
  {
    Id = Guard.Against.Default(id, nameof(id));
    Key = Guard.Against.NullOrWhiteSpace(key, nameof(key));
  }

  /// <summary>
  /// Privátní konstruktor pro EF Core.
  /// </summary>
  private ActivationKey()
  {
    // EF Core vyžaduje bezparametrový konstruktor
  }

  /// <summary>
  /// Hodnota aktivačního klíče.
  /// </summary>
  public string Key { get; private set; } = string.Empty;

  /// <summary>
  /// Aktualizuje hodnotu aktivačního klíče.
  /// </summary>
  /// <param name="newKey">Nová hodnota klíče.</param>
  /// <returns>Aktuální instance pro fluent API.</returns>
  public ActivationKey UpdateKey(string newKey)
  {
    Key = Guard.Against.NullOrWhiteSpace(newKey, nameof(newKey));
    return this;
  }
}
