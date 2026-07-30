using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.DataModelRecords.Update;

public record UpdateDataModelRecordCommand(Datasource Datasource, DataModelRecordDTO Record) : ICommand<Result<Guid>>;
