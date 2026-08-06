using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.UseCases.DataModels.DeleteAll;

/// <summary>
/// Příkaz pro smazání všech datových modelů z lokální databáze včetně jejich záznamů (DataModelRecord).
/// </summary>
public record DeleteAllDataModelsCommand : ICommand<Result<DeleteAllDataModelsResponse>>;
