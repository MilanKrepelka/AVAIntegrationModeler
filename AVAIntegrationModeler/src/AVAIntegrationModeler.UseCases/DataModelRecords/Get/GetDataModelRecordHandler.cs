using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.DataModelRecordAggregate.Specifications;

namespace AVAIntegrationModeler.UseCases.DataModelRecords.Get;

public class GetDataModelRecordHandler(IDataModelRecordQueryService queryService)
  : IQueryHandler<GetDataModelRecordQuery, Result<DataModelRecordDTO>>
{
  public async Task<Result<DataModelRecordDTO>> Handle(GetDataModelRecordQuery request, CancellationToken cancellationToken)
  {
    var record = await queryService.GetByIdAsync(request.Datasource, request.RecordId, cancellationToken);
    if (record is null) return Result<DataModelRecordDTO>.NotFound();
    return Result<DataModelRecordDTO>.Success(record);
  }
}
