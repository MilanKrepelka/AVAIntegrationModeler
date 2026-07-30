using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.UseCases.DataModelRecords.Delete;

public record DeleteDataModelRecordCommand(Datasource Datasource, Guid RecordId) : ICommand<Result>;
