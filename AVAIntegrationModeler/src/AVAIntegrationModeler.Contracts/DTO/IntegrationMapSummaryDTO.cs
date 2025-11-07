using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Contracts.DTO;
/// <summary>
/// DTO pro integrační mapu.
/// </summary>
public class IntegrationMapSummaryDTO
{
  /// <summary>
  /// Identifikátor mapy.
  /// </summary>
  public Guid Id { get; set; }

  /// <summary>
  /// Název mapy.
  /// </summary>
  public string Area { get; set; } = string.Empty;

  /// <summary>
  /// Položky integrace v mapě.
  /// </summary>
  public List<IntegrationMapSummaryItemDTO> Items { get; set; } = new();


}
