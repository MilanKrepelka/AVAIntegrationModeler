using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.DataModels.List;

public class ListDataModelsHandler(IListDataModelQueryService _query)
  : IQueryHandler<ListDataModelsQuery, Result<IEnumerable<DataModelDTO>>>
{
  public async Task<Result<IEnumerable<DataModelDTO>>> Handle(ListDataModelsQuery request, CancellationToken cancellationToken)
  {
    var result = await _query.ListAsync(request.Datasource);
    return Result.Success(result);
  }
}
