using Ardalis.GuardClauses;
using AVAIntegrationModeler.Core.ScenarioAggregate;
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

    existingScenario.SetName(request.Scenario.Name);
    existingScenario.SetDescription(request.Scenario.Description);
    existingScenario.SetCode(request.Scenario.Code);

    existingScenario.SetInputFeature(new Feature(request?.Scenario.InputFeatureId));
    existingScenario.SetOutputFeature(new Feature(request?.Scenario.OutputFeatureId));

    await _repository.UpdateAsync(existingScenario, cancellationToken);
    
    return new Result<ScenarioDTO>(request?.Scenario!);
  }
}
