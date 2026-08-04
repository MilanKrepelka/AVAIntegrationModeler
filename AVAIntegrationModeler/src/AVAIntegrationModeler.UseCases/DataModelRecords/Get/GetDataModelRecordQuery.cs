using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.DataModelRecords.Get;

public record GetDataModelRecordQuery(Datasource Datasource, Guid RecordId) : IQuery<Result<DataModelRecordDTO>>;
