namespace AVAIntegrationModeler.UseCases.DataModels.Import;

public record ImportDataModelCommand(Guid AvaPlaceModelId) : ICommand<Result<Guid>>;
