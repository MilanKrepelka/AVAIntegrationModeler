using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Contracts.DTO;

/// <summary>
/// DTO pro položku integrační mapy - reprezentuje jeden scénář a jeho aktivační klíče.
/// </summary>
public record IntegrationMapSummaryItemDTO
{
  /// <summary>
  /// Jedinečný identifikátor položky.
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Identifikátor scénáře spojeného s touto položkou.
  /// </summary>
  public required Guid ScenarioId { get; init; }

}
