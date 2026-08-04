namespace AVAIntegrationModeler.UseCases.Deployments.Delete;

/// <summary>
/// Příkaz pro smazání nasazení.
/// </summary>
/// <param name="Code">Kód nasazení.</param>
public record DeleteDeploymentCommand(string Code) : Ardalis.SharedKernel.ICommand<Result>;
