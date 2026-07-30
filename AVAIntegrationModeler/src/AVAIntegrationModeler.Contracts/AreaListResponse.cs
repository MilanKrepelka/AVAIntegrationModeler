using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Contracts.Areas;

/// <summary>
/// Odpověď na požadavek <see cref="AreaListRequest"/>.
/// </summary>
public class AreaListResponse
{
  /// <summary>
  /// Seznam <see cref="AreaDTO"/>.
  /// </summary>
  public List<AreaDTO> Areas { get; set; } = [];
}
