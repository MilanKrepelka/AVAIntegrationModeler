using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Deployments.CompleteDevelopment;

/// <summary>
/// Příkaz pro ukončení vývoje nasazení — nastaví datum posledního nasazení na aktuální okamžik a nasazení uloží.
/// </summary>
/// <param name="Code">Kód nasazení.</param>
public record CompleteDeploymentDevelopmentCommand(string Code) : Ardalis.SharedKernel.ICommand<Result<DeploymentDTO>>;
