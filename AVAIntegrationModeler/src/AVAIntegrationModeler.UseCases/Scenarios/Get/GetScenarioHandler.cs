﻿using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.ContributorAggregate;
using AVAIntegrationModeler.Domain.ContributorAggregate.Specifications;
using AVAIntegrationModeler.Domain.ScenarioAggregate;
using AVAIntegrationModeler.Domain.ScenarioAggregate.Specifications;
using AVAIntegrationModeler.UseCases.Contributors;
using AVAIntegrationModeler.UseCases.Contributors.Get;
using AVAIntegrationModeler.UseCases.Scenarios.Mapping;

namespace AVAIntegrationModeler.UseCases.Scenarios.Get;
/// <summary>
/// Queries don't necessarily need to use repository methods, but they can if it's convenient
/// </summary>
public class GetScenarioHandler(IReadRepository<Scenario> _repository)
  : IQueryHandler<GetScenarioQuery, Result<ScenarioDTO>>
{
  public async Task<Result<ScenarioDTO>> Handle(GetScenarioQuery request, CancellationToken cancellationToken)
  {
    var spec = new ScenarioByIdSpec(request.ScenarioId);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null) return Result.NotFound();

    return ScenarioMapper.MapToDTO(entity);
  }
}

