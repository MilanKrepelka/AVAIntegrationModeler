using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Scenarios.Update;


/// <summary>
/// Příkaz pro aktualizaci integračního scénáře.
/// </summary>
/// <param name="datasource">Datový zdroj, ve kterém se scénář nachází.</param>
/// <param name="Scenario">Aktualizovaný integrační scénář.</param>
public record UpdateScenarioCommand(Datasource datasource, ScenarioDTO Scenario) : ICommand<Result<ScenarioDTO>>;
