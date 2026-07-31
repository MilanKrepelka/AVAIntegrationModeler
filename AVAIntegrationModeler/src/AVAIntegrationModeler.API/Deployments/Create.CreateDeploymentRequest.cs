using System.ComponentModel.DataAnnotations;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.API.Deployments;

/// <summary>
/// Požadavek na vytvoření nasazení.
/// </summary>
public class CreateDeploymentRequest
{
  public const string Route = "/Deployments";

  [Required]
  public DeploymentDTO Deployment { get; set; } = new DeploymentDTO();
}
