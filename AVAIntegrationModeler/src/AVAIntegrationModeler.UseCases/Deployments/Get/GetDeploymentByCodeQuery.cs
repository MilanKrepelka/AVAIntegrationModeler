using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Deployments.Get;

/// <summary>
/// Dotaz pro získání nasazení podle kódu.
/// </summary>
/// <param name="Code">Kód nasazení.</param>
public record GetDeploymentByCodeQuery(string Code) : Ardalis.SharedKernel.IQuery<Result<DeploymentDTO>>;
