namespace AVAIntegrationModeler.UseCases.Scenarios.Update;


/// <summary>
/// Příkaz pro aktualizaci integračním scénáři.
/// </summary>
/// <param name="ContributorId">Identifikátor přispěvatele, který má být aktualizován.</param>
/// <param name="NewName">Nové jméno přispěvatele.</param>
public record UpdateScenarioCommand(Scenarios.ScenarioDTO Scenario) : ICommand<Result<ScenarioDTO>>;
