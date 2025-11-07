using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.IntegrationMaps.List;

public class ListIntegrationMapsHandler(IListIntegrationMapsQueryService _query)
  : IQueryHandler<ListIntegrationMapsQuery, Result<IEnumerable<IntegrationMapSummaryDTO>>>
{
  public async Task<Result<IEnumerable<IntegrationMapSummaryDTO>>> Handle(ListIntegrationMapsQuery request, CancellationToken cancellationToken)
  {
    var result = await _query.ListSummaryAsync(request.Datasource);

    return Result.Success(result);
  }
}
