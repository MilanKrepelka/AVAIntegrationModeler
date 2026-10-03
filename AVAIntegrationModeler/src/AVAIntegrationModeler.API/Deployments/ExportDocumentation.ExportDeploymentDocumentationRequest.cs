namespace AVAIntegrationModeler.API.Deployments;

/// <summary>
/// Požadavek na export markdown dokumentace nasazení jako ZIP archívu.
/// </summary>
public class ExportDeploymentDocumentationRequest
{
  public const string Route = "/Deployments/{DeploymentCode}/export-documentation";

  public string DeploymentCode { get; set; } = string.Empty;

  [QueryParam]
  public int Months { get; set; } = 12;
}
