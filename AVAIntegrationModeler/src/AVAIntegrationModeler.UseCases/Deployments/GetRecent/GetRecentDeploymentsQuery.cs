using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Deployments.GetRecent;

/// <summary>
/// Dotaz pro výpis posledních <see cref="Count"/> nasazení.
/// </summary>
public record GetRecentDeploymentsQuery(int Count = 5)
  : Ardalis.SharedKernel.IQuery<Result<IEnumerable<DeploymentDTO>>>;
