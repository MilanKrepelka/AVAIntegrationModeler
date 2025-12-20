namespace AVAIntegrationModeler.UseCases.Scenarios.Delete;

public record DeleteScenarioCommand(string ScenarioCode) : ICommand<Result>;
