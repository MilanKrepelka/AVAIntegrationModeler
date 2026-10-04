namespace AVAIntegrationModeler.API.Deployments;

/// <summary>
/// Požadavek na ukončení vývoje nasazení.
/// </summary>
public class CompleteDeploymentDevelopmentRequest
{
  public const string Route = "/Deployments/{DeploymentCode}/complete-development";

  /// <summary>
  /// Kód nasazení.
  /// </summary>
  public string DeploymentCode { get; set; } = string.Empty;
}
