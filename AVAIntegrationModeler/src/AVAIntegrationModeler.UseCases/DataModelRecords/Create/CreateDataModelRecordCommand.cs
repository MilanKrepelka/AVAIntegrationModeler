using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.DataModelRecords.Create;

public record CreateDataModelRecordCommand(Datasource Datasource, DataModelRecordDTO Record) : ICommand<Result<Guid>>;
