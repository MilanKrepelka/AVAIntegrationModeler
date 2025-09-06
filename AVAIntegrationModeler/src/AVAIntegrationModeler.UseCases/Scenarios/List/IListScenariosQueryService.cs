namespace AVAIntegrationModeler.UseCases.Scenarios.List;

/// <summary>
/// Represents a service that will actually fetch the necessary data
/// Typically implemented in Infrastructure
/// </summary>
public interface IListScenariosQueryService
{
  Task<IEnumerable<ScenarioDTO>> ListAsync();
}
