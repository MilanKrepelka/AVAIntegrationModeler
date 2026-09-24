using AVAIntegrationModeler.UseCases.Export;

namespace AVAIntegrationModeler.UseCases.Deployments.ExportDocumentation;

/// <summary>
/// Dotaz pro export markdown dokumentace nasazení jako ZIP archívu.
/// </summary>
/// <param name="DeploymentCode">Kód nasazení</param>
/// <param name="Months">Počet měsíců pro generování dokumentů (výchozí 12)</param>
public record ExportDeploymentDocumentationQuery(string DeploymentCode, int Months = 12)
  : IQuery<Result<ExportResult<string>>>;
