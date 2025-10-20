namespace AVAIntegrationModeler.UseCases.Scenarios.Delete;

public record DeleteScenarioCommand(Guid ScenarioId) : ICommand<Result>;
