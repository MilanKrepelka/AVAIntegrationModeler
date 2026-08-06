using AVAIntegrationModeler.Domain.DataModelAggregate;

namespace AVAIntegrationModeler.UseCases.DataModels;

public interface IDataModelRepository : IRepository<DataModel>
{
  Task SyncFieldsAndSaveAsync(
    DataModel model,
    IList<DataModelField> fieldsToDelete,
    IList<DataModelField> fieldsToAdd,
    CancellationToken ct = default);

  /// <summary>
  /// Smaže všechny datové modely včetně jejich polí a referencí na entity typy.
  /// </summary>
  /// <returns>Počet smazaných datových modelů.</returns>
  Task<int> DeleteAllAsync(CancellationToken ct = default);
}
