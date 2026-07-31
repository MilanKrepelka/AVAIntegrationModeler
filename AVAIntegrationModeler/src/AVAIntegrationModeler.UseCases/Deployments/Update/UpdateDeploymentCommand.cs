using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Deployments.Update;

/// <summary>
/// Příkaz pro aktualizaci nasazení.
/// </summary>
/// <param name="Deployment"><see cref="DeploymentDTO"/></param>
public record UpdateDeploymentCommand(DeploymentDTO Deployment) : Ardalis.SharedKernel.ICommand<Result<DeploymentDTO>>;
