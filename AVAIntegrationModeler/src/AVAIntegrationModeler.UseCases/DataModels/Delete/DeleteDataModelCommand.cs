using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.UseCases.DataModels.Delete;

public record DeleteDataModelCommand(Datasource Datasource, Guid DataModelId) : Ardalis.SharedKernel.ICommand<Result>;
