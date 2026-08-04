using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.DataModelRecords;

public interface IDataModelRecordQueryService : ICacheableQueryService
{
  Task<IEnumerable<DataModelRecordDTO>> ListAsync(Datasource datasource, Guid? modelId = null, int? skip = null, int? take = null, CancellationToken cancellationToken = default);
  Task<DataModelRecordDTO?> GetByIdAsync(Datasource datasource, Guid id, CancellationToken cancellationToken = default);
}
