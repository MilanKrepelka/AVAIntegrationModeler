using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Deployments.Create;

/// <summary>
/// Příkaz pro vytvoření nového nasazení.
/// </summary>
/// <param name="Deployment"><see cref="DeploymentDTO"/></param>
public record CreateDeploymentCommand(DeploymentDTO Deployment) : Ardalis.SharedKernel.ICommand<Result<Guid>>;
