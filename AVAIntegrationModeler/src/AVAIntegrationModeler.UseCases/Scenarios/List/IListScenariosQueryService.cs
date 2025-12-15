using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Scenarios.List;

/// <summary>
/// Represents a service that will actually fetch the necessary data
/// Typically implemented in Infrastructure
/// </summary>
public interface IListScenariosQueryService
{
  Task<IEnumerable<ScenarioDTO>> ListAsync(Contracts.Datasource dataSource);

  /// <summary>
  /// Vrátí detail scénáře
  /// </summary>
  /// <param name="dataSource"><see cref="Contracts.Datasource"/></param>
  /// <param name="scenarioId">Identifikátor scénáře</param>
  /// <returns>Integrační scénář</returns>
  Task<ScenarioDTO> GetScenario(Contracts.Datasource dataSource, Guid scenarioId);
}
