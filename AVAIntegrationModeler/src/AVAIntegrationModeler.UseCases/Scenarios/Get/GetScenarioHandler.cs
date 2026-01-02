using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.ContributorAggregate;
using AVAIntegrationModeler.Domain.ContributorAggregate.Specifications;
using AVAIntegrationModeler.Domain.ScenarioAggregate;
using AVAIntegrationModeler.Domain.ScenarioAggregate.Specifications;
using AVAIntegrationModeler.UseCases.Contributors;
using AVAIntegrationModeler.UseCases.Contributors.Get;
using AVAIntegrationModeler.UseCases.Scenarios.List;
using AVAIntegrationModeler.UseCases.Scenarios.Mapping;

namespace AVAIntegrationModeler.UseCases.Scenarios.Get;
/// <summary>
/// Queries don't necessarily need to use repository methods, but they can if it's convenient
/// </summary>
public class GetScenarioHandler(IListScenariosQueryService query)
  : IQueryHandler<GetScenarioQuery, Result<ScenarioDTO>>, IQueryHandler<GetScenarioByCodeQuery, Result<ScenarioDTO>>
{
  public async Task<Result<ScenarioDTO>> Handle(GetScenarioQuery request, CancellationToken cancellationToken)
  {
    var scenario = await query.GetScenario(request.Datasource, request.ScenarioId, cancellationToken);
    if (scenario == null) return Result.NotFound();
    return Result.Success(scenario);
  }

  public async Task<Result<ScenarioDTO>> Handle(GetScenarioByCodeQuery request, CancellationToken cancellationToken)
  {
    var scenario = await query.GetScenario(request.Datasource, request.ScenarioCode, cancellationToken);
    
    if (scenario.InputFeatureId.HasValue)
    {
      //var inputFeatureSpec = new GetFeatureByIdSpecification(request.Datasource, scenario.InputFeatureId.Value);
      //var inputFeature = await query.GetFeature(inputFeatureSpec, cancellationToken);
      //scenario = scenario with { InputFeatureSummary = inputFeature };
    }
    if (scenario == null) return Result.NotFound();
    return Result.Success(scenario);
  }
}

