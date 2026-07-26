using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.DataModels.Create;

public record CreateDataModelCommand(
    Datasource Datasource,
    DataModelDTO DataModel
) : Ardalis.SharedKernel.ICommand<Result<Guid>>;
