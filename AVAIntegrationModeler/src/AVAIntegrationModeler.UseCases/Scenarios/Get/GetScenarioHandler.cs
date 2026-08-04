using Ardalis.GuardClauses;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Scenarios.Mapping;

namespace AVAIntegrationModeler.UseCases.Scenarios.Get;

/// <summary>
/// Queries don't necessarily need to use repository methods, but they can if it's convenient
/// </summary>
public class GetScenarioHandler(IScenariosQueryService query)
  : IQueryHandler<GetScenarioQuery, Result<ScenarioDTO>>, IQueryHandler<GetScenarioByCodeQuery, Result<ScenarioDTO>>
{
  public async Task<Result<ScenarioDTO>> Handle(GetScenarioQuery request, CancellationToken cancellationToken)
  {
    try
    {
      var scenario = await query.GetScenario(request.Datasource, request.ScenarioId, cancellationToken);
      return Result.Success(scenario);
    }
    catch (NotFoundException)
    {
      return Result.NotFound();
    }
  }

  public async Task<Result<ScenarioDTO>> Handle(GetScenarioByCodeQuery request, CancellationToken cancellationToken)
  {
    try
    {
      var scenario = await query.GetScenario(request.Datasource, request.ScenarioCode, cancellationToken);
      return Result.Success(scenario);
    }
    catch (NotFoundException)
    {
      return Result.NotFound();
    }
  }
}

