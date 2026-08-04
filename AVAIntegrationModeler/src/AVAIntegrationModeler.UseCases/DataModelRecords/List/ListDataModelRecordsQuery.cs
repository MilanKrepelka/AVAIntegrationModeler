using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.DataModelRecords.List;

public record ListDataModelRecordsQuery(Datasource Datasource, Guid? ModelId = null, int? Skip = null, int? Take = null)
  : IQuery<Result<IEnumerable<DataModelRecordDTO>>>;
