using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Deployments.List;

/// <summary>
/// Dotaz pro výpis všech nasazení.
/// </summary>
public record ListDeploymentsQuery() : Ardalis.SharedKernel.IQuery<Result<IEnumerable<DeploymentDTO>>>;
