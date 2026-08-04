using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.DataModels.Get;

public record GetDataModelQuery(Datasource Datasource, Guid DataModelId) : Ardalis.SharedKernel.IQuery<Result<DataModelDTO>>;
