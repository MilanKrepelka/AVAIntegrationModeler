using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.UseCases.DataModels.Import;

/// <summary>
/// Příkaz pro hromadný import všech datových modelů z AVAPlace do lokální databáze.
/// </summary>
public record ImportAllDataModelsCommand : ICommand<Result<ImportAllDataModelsFromAvaPlaceResponse>>;
