using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.DataModels.Update;

public record UpdateDataModelCommand(
    Datasource Datasource,
    DataModelDTO DataModel
) : Ardalis.SharedKernel.ICommand<Result<DataModelDTO>>;
