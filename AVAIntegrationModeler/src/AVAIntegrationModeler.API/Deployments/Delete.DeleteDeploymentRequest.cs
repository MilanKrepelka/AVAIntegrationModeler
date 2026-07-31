namespace AVAIntegrationModeler.API.Deployments;

/// <summary>
/// Požadavek na smazání nasazení.
/// </summary>
public class DeleteDeploymentRequest
{
  public const string Route = "/Deployments/{DeploymentCode}";

  public string DeploymentCode { get; set; } = string.Empty;
}
