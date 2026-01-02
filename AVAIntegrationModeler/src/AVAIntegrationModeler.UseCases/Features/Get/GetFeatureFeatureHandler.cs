using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.ContributorAggregate;
using AVAIntegrationModeler.Domain.ContributorAggregate.Specifications;
using AVAIntegrationModeler.Domain.ScenarioAggregate;
using AVAIntegrationModeler.Domain.ScenarioAggregate.Specifications;
using AVAIntegrationModeler.UseCases.Contributors;
using AVAIntegrationModeler.UseCases.Contributors.Get;
using AVAIntegrationModeler.UseCases.Features.Mapping;

namespace AVAIntegrationModeler.UseCases.Features.Get;
/// <summary>
/// Queries don't necessarily need to use repository methods, but they can if it's convenient
/// </summary>
public class GetFeatureFeatureHandler(IFeaturesQueryService query)
  : IQueryHandler<GetFeatureQuery, Result<FeatureDTO>>, IQueryHandler<GetFeatureByCodeQuery, Result<FeatureDTO>>
{
  public async Task<Result<FeatureDTO>> Handle(GetFeatureQuery request, CancellationToken cancellationToken)
  {
    var feature = await query.GetFeature(request.Datasource, request.FeatureId, cancellationToken);
    if (feature == null) return Result.NotFound();
    return Result.Success(feature);
  }

  public async Task<Result<FeatureDTO>> Handle(GetFeatureByCodeQuery request, CancellationToken cancellationToken)
  {
    var feature = await query.GetFeature(request.Datasource, request.FeatureCode, cancellationToken);

    //if (feature.InputFeatureId.HasValue)
    //{
    //  //var inputFeatureSpec = new GetFeatureByIdSpecification(request.Datasource, scenario.InputFeatureId.Value);
    //  //var inputFeature = await query.GetFeature(inputFeatureSpec, cancellationToken);
    //  //scenario = scenario with { InputFeatureSummary = inputFeature };
    //}
    if (feature == null) return Result.NotFound();
    return Result.Success(feature);
  }
}

