using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Scenarios.Get;

public record GetScenarioQuery(Datasource Datasource, Guid ScenarioId) : IQuery<Result<ScenarioDTO>>;
