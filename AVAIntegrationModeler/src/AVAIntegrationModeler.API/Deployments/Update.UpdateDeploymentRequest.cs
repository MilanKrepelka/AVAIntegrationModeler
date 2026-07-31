using System.ComponentModel.DataAnnotations;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.API.Deployments;

/// <summary>
/// Požadavek na aktualizaci nasazení.
/// </summary>
public class UpdateDeploymentRequest
{
  public const string Route = "/Deployments/{DeploymentCode}";

  public string DeploymentCode { get; set; } = string.Empty;

  [Required]
  public DeploymentDTO Deployment { get; set; } = new DeploymentDTO();
}
