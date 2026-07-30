using AVAIntegrationModeler.Domain.DataModelAggregate;

namespace AVAIntegrationModeler.UseCases.DataModels;

public interface IDataModelRepository : IRepository<DataModel>
{
  Task SyncFieldsAndSaveAsync(
    DataModel model,
    IList<DataModelField> fieldsToDelete,
    IList<DataModelField> fieldsToAdd,
    CancellationToken ct = default);
}
