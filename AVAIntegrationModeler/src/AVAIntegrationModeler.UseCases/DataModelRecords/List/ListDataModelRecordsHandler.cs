using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.DataModelRecords.List;

public class ListDataModelRecordsHandler(IDataModelRecordQueryService queryService)
  : IQueryHandler<ListDataModelRecordsQuery, Result<IEnumerable<DataModelRecordDTO>>>
{
  public async Task<Result<IEnumerable<DataModelRecordDTO>>> Handle(ListDataModelRecordsQuery request, CancellationToken cancellationToken)
  {
    if (request.Datasource != Datasource.Database)
      return Result<IEnumerable<DataModelRecordDTO>>.Invalid(
        new ValidationError("Datasource", "Záznamy datových modelů jsou dostupné pouze pro zdroj Database."));

    var records = await queryService.ListAsync(request.Datasource, request.ModelId, request.Skip, request.Take, cancellationToken);
    return Result<IEnumerable<DataModelRecordDTO>>.Success(records);
  }
}
