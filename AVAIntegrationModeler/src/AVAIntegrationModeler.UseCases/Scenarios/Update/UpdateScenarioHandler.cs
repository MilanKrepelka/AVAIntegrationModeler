using Ardalis.GuardClauses;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.ScenarioAggregate;
using AVAIntegrationModeler.UseCases.Scenarios.Mapping;
using AVAIntegrationModeler.UseCases.Scenarios.Update;

namespace AVAIntegrationModeler.UseCases.Scenarios.Update;

public class UpdateScenarioHandler(
  IRepository<Scenario> _repository

  )
  : ICommandHandler<UpdateScenarioCommand, Result<ScenarioDTO>>
{
  public async Task<Result<ScenarioDTO>> Handle(UpdateScenarioCommand request, CancellationToken cancellationToken)
  {
    Guard.Against.Null(request, nameof(request));
    Guard.Against.Null(request.Scenario, nameof(request.Scenario));

    var existingScenario = await _repository.GetByIdAsync(request.Scenario.Id, cancellationToken);
    if (existingScenario == null)
    {
      return Result.NotFound();
    }

    existingScenario.SetName(LocalizedValueMapper.MapToEntity(request.Scenario.Name));
    existingScenario.SetDescription(LocalizedValueMapper.MapToEntity(request.Scenario.Description));
    existingScenario.SetCode(request.Scenario.Code);

    existingScenario.SetInputFeature(request?.Scenario.InputFeatureId);
    existingScenario.SetOutputFeature(request?.Scenario.OutputFeatureId);

    await _repository.UpdateAsync(existingScenario, cancellationToken);
    
    return new Result<ScenarioDTO>(request?.Scenario!);
  }
}
