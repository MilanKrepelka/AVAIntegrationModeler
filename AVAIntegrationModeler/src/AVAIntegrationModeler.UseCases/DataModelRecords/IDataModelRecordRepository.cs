using AVAIntegrationModeler.Domain.DataModelRecordAggregate;

namespace AVAIntegrationModeler.UseCases.DataModelRecords;

public interface IDataModelRecordRepository : IRepository<DataModelRecord>
{
  Task SyncFieldsAndSaveAsync(
    DataModelRecord record,
    IList<DataModelRecordField> fieldsToDelete,
    IList<DataModelRecordField> fieldsToAdd,
    CancellationToken ct = default);
}
