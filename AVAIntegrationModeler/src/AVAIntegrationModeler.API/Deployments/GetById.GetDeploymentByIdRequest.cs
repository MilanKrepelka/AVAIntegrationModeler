namespace AVAIntegrationModeler.API.Deployments;

/// <summary>
/// Požadavek na získání nasazení podle identifikátoru.
/// </summary>
public class GetDeploymentByIdRequest
{
  public const string Route = "/Deployments/by-id/{DeploymentId:guid}";

  public Guid DeploymentId { get; set; }
}
