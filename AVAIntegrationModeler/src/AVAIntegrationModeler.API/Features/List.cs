using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Features;
using AVAIntegrationModeler.UseCases.Features.List;

namespace AVAIntegrationModeler.API.Features;

/// <summary>
/// List all Features
/// </summary>
/// <remarks>
/// List all contributors - returns a ContributorListResponse containing the Features.
/// </remarks>
public class List(IMediator _mediator) : Endpoint<FeatureListRequest, FeatureListResponse>
{
  public override void Configure()
  {
    Get("/Features");
    AllowAnonymous();
    Options(x => x.CacheOutput(p => p.Expire(TimeSpan.FromSeconds(5))));
  }

  public override async Task HandleAsync(FeatureListRequest request, CancellationToken cancellationToken)
  {

    Result<IEnumerable<FeatureDTO>> result = await _mediator.Send(new ListFeaturesQuery(request.Datasource, null, null), cancellationToken);

    var result2 = await new ListFeaturesQuery2(request.Datasource, null, null)
      .ExecuteAsync(cancellationToken);

    if (result.IsSuccess)
    {
      Response = new FeatureListResponse
      {
        Features = result.Value.ToList()
      };
    }
  }
}
