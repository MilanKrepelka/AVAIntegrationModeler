namespace AVAIntegrationModeler.API.Deployments;

/// <summary>
/// Požadavek na nahrání DataModelů nasazení včetně jejich záznamů do AVAPlace.
/// </summary>
public class UploadDeploymentToAvaPlaceRequest
{
  public const string Route = "/Deployments/{DeploymentCode}/upload-to-avaplace";

  public string DeploymentCode { get; set; } = string.Empty;
}
