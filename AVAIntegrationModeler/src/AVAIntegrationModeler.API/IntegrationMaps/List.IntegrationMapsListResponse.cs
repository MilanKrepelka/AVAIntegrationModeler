using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.IntegrationMapAggregate;


namespace AVAIntegrationModeler.API.IntegrationMaps;

/// <summary>
/// Odpověď na požadavek <see cref="IntegrationMapListRequest"/>
/// </summary>
public class IntegrationMapListResponse
{
  /// <summary>
  /// Seznam <see cref="IntegrationMapSummaryDTO"/>
  /// </summary>
  public List<IntegrationMapSummaryDTO> IntegrationMaps { get; set; } = [];
}
