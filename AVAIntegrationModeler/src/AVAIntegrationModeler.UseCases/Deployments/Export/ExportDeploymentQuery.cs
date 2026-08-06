using AVAIntegrationModeler.UseCases.Export;

namespace AVAIntegrationModeler.UseCases.Deployments.Export;

/// <summary>
/// Dotaz pro export DataModelů a jejich záznamů pro dané nasazení.
/// </summary>
public record ExportDeploymentQuery(string DeploymentCode)
  : IQuery<Result<ExportResult<object>>>;
