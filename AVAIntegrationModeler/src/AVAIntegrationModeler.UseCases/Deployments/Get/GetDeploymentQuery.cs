using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Deployments.Get;

/// <summary>
/// Dotaz pro získání nasazení podle identifikátoru.
/// </summary>
/// <param name="Id">Identifikátor nasazení.</param>
public record GetDeploymentQuery(Guid Id) : Ardalis.SharedKernel.IQuery<Result<DeploymentDTO>>;
