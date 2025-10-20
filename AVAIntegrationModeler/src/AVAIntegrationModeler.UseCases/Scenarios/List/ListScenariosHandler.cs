using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Scenarios.List;

public class ListScenariosHandler(IListScenariosQueryService _query)
  : IQueryHandler<ListScenariosQuery, Result<IEnumerable<ScenarioDTO>>>
{
  public async Task<Result<IEnumerable<ScenarioDTO>>> Handle(ListScenariosQuery request, CancellationToken cancellationToken)
  {
    var result = await _query.ListAsync(request.Datasource);

    return Result.Success(result);
  }
}
