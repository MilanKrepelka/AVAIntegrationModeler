using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Export;

namespace AVAIntegrationModeler.UseCases.DataModelRecords.Export;

public record ExportDataModelRecordsQuery(Datasource Datasource, List<Guid> RecordIds)
  : IQuery<Result<ExportResult<List<DataModelRecordDTO>>>>;
