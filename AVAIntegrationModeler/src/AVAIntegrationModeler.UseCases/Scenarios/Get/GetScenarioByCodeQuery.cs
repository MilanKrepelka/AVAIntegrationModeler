using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Scenarios.Get;

public record GetScenarioByCodeQuery(Datasource Datasource, string ScenarioCode) : IQuery<Result<ScenarioDTO>>;
