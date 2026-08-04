namespace AVAIntegrationModeler.API.Deployments;

/// <summary>
/// Požadavek na získání nasazení podle kódu.
/// </summary>
public class GetDeploymentByCodeRequest
{
  public const string Route = "/Deployments/{DeploymentCode}";

  public string DeploymentCode { get; set; } = string.Empty;
}
