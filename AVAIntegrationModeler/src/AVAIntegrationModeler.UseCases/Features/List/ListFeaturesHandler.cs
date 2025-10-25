using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Features.List;

public class ListFeaturesHandler(IListFeaturesQueryService _query)
  : IQueryHandler<ListFeaturesQuery, Result<IEnumerable<FeatureDTO>>>
{
  public async Task<Result<IEnumerable<FeatureDTO>>> Handle(ListFeaturesQuery request, CancellationToken cancellationToken)
  {
    var result = await _query.ListAsync(request.Datasource);

    return Result.Success(result);
  }
}
