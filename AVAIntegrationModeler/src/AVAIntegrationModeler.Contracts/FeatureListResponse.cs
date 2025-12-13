using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Contracts;

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
