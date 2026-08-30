namespace AVAIntegrationModeler.Contracts.DataModels;

/// <summary>
/// Odpověď se souhrnem porovnání DataModelů nasazení.
/// </summary>
public class GetDeploymentChangesSummaryResponse
{
  /// <summary>Výsledek souhrnného porovnání.</summary>
  public DeploymentChangesSummaryDTO? Summary { get; set; }
}
