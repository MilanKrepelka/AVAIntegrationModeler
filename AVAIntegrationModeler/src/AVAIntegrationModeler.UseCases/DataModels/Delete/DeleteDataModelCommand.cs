using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.UseCases.DataModels.Delete;

public record DeleteDataModelCommand(Guid DataModelId) : Ardalis.SharedKernel.ICommand<Result>;
