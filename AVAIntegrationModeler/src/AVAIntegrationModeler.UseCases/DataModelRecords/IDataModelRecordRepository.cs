using AVAIntegrationModeler.Domain.DataModelRecordAggregate;

namespace AVAIntegrationModeler.UseCases.DataModelRecords;

public interface IDataModelRecordRepository : IRepository<DataModelRecord>
{
  Task SyncFieldsAndSaveAsync(
    DataModelRecord record,
    IList<DataModelRecordField> fieldsToDelete,
    IList<DataModelRecordField> fieldsToAdd,
    CancellationToken ct = default);

  /// <summary>
  /// Smaže všechny záznamy datových modelů včetně jejich polí.
  /// </summary>
  /// <returns>Počet smazaných záznamů.</returns>
  Task<int> DeleteAllAsync(CancellationToken ct = default);
}
