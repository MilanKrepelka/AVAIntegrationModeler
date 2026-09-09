namespace AVAIntegrationModeler.Contracts.DataModels;

/// <summary>
/// Požadavek na souhrnné porovnání DataModelů nasazení.
/// DataModelIds jsou předány přímo — nevyžaduje DB lookup nasazení.
/// </summary>
public class GetDeploymentChangesSummaryRequest
{
  /// <summary>Kód nasazení (pouze informativní, pro výstup).</summary>
  public string DeploymentCode { get; set; } = string.Empty;
  /// <summary>Název nasazení (pouze informativní, pro výstup).</summary>
  public string DeploymentName { get; set; } = string.Empty;
  /// <summary>Identifikátory DataModelů, které jsou součástí nasazení.</summary>
  public List<Guid> DataModelIds { get; set; } = new();
}
