using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Areas.List;

/// <summary>
/// Handler pro dotaz na výpis oblastí.
/// </summary>
public class ListAreasHandler(IAreasQueryService query)
  : IQueryHandler<ListAreasQuery, Result<IEnumerable<AreaDTO>>>
{
  public async Task<Result<IEnumerable<AreaDTO>>> Handle(ListAreasQuery request, CancellationToken cancellationToken)
  {
    var result = await query.ListAsync(request.Datasource);
    return Result.Success(result);
  }
}
