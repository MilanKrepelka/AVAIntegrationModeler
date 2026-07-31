using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Contracts.Deployments;

/// <summary>
/// Odpověď na požadavek <see cref="DeploymentListRequest"/>.
/// </summary>
public class DeploymentListResponse
{
  /// <summary>
  /// Seznam <see cref="DeploymentDTO"/>.
  /// </summary>
  public List<DeploymentDTO> Deployments { get; set; } = [];
}
