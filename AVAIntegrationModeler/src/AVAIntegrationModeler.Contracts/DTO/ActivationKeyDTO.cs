using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Contracts.DTO;

/// <summary>
/// DTO pro aktivační klíč integrační položky.
/// </summary>
/// <remarks>
/// Aktivační klíč určuje, za jakých podmínek (klíčů) se má daný integrační scénář spustit.
/// Například klíč může být "ORDER_CREATED", "CUSTOMER_UPDATED" apod.
/// </remarks>
public record ActivationKeyDTO
{
  /// <summary>
  /// Jedinečný identifikátor klíče.
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Hodnota aktivačního klíče.
  /// </summary>
  public string Key { get; init; } = string.Empty;
}
