using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Core.ScenarioAggregate;

namespace AVAIntegrationModeler.API.Features;

/// <summary>
/// Odpověď na požadavek <see cref="FeatureListRequest"/>
/// </summary>
public class FeatureListResponse
{
  /// <summary>
  /// Seznam <see cref="FeatureDTO"/>
  /// </summary>
  public List<FeatureDTO> Features { get; set; } = [];
}
