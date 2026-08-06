using Ardalis.GuardClauses;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain;
using AVAIntegrationModeler.Domain.ScenarioAggregate;
using AVAIntegrationModeler.UseCases.Scenarios.Update;

namespace AVAIntegrationModeler.UseCases.Scenarios.Update;

public class UpdateScenarioHandler(
  IRepository<Scenario> _repository,
  IScenariosQueryService _scenariosQueryService,
  IDomainEntityValidationService<Scenario> _scenarioValidationService
  )
  : ICommandHandler<UpdateScenarioCommand, Result<ScenarioDTO>>
{
  public async Task<Result<ScenarioDTO>> Handle(UpdateScenarioCommand request, CancellationToken cancellationToken)
  {
    //TODO: chybí odbočka pro AVAPlace integration
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

    var validationResult = await _scenarioValidationService.Validate(request!.datasource, existingScenario, cancellationToken);
    if (!validationResult.IsSuccess)
      return validationResult;

    await _repository.UpdateAsync(existingScenario, cancellationToken);
    _scenariosQueryService.InvalidateCache(request!.datasource);
    return new Result<ScenarioDTO>(request?.Scenario!);
  }
}
