using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Scenarios.Get;

public record GetScenarioQuery(Guid ScenarioId) : IQuery<Result<ScenarioDTO>>;
