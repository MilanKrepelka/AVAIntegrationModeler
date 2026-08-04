namespace AVAIntegrationModeler.API.Deployments;

/// <summary>
/// Požadavek na export nasazení — DataModely a záznamy jako ZIP archív.
/// </summary>
public class ExportDeploymentRequest
{
  public const string Route = "/Deployments/{DeploymentCode}/export";

  public string DeploymentCode { get; set; } = string.Empty;
}
