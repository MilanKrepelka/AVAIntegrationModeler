using AVAIntegrationModeler.Core.ScenarioAggregate;
using AVAIntegrationModeler.Core.ValueObjects;
using Ardalis.SharedKernel;

namespace AVAIntegrationModeler.UseCases.Scenarios.Create;

public class CreateScenarioHandler(IRepository<Scenario> _scenarioRepository)
    : ICommandHandler<CreateScenarioCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateScenarioCommand request, CancellationToken cancellationToken)
  {
    var scenario = new Scenario(request.Id)
        .SetCode(request.Code)
        .SetName(request.Name)
        .SetDescription(request.Decsription)
        .SetInputFeature(request.InputFeatureId)
        .SetOutputFeature(request.OutputFeatureId);
    var created = await _scenarioRepository.AddAsync(scenario, cancellationToken);

    if (created == null)
    {
      return Result.Error("Scenario was not created.");
    }
    return new Result<Guid>(created.Id);
  }
}


