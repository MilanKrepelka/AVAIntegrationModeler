using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Contracts.DTO;

/// <summary>
/// DTO pro položku integrační mapy - reprezentuje jeden scénář a jeho aktivační klíče.
/// </summary>
public record IntegrationMapItemDTO
{
  /// <summary>
  /// Jedinečný identifikátor položky.
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Identifikátor scénáře spojeného s touto položkou.
  /// </summary>
  public Guid ScenarioId { get; init; }

  /// <summary>
  /// Seznam aktivačních klíčů pro tento scénář.
  /// </summary>
  public List<ActivationKeyDTO> Keys { get; init; } = new();

  
}
