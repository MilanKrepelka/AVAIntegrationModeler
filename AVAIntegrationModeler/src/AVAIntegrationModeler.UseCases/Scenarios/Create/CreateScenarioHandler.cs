using AVAIntegrationModeler.Core.ScenarioAggregate;
using AVAIntegrationModeler.Core.ValueObjects;
using Ardalis.SharedKernel;

namespace AVAIntegrationModeler.UseCases.Scenarios.Create;

public class CreateScenarioHandler(IRepository<Scenario> _scenarioRepository)
    : ICommandHandler<CreateScenarioCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateScenarioCommand request, CancellationToken cancellationToken)
    {
        var scenario = new Scenario(request.Code)
            .SetName(request.Name)
            .SetDescription(request.Decsription)
            .SetInputFeature(new Feature( request.InputFeatureId))
            .SetOutputFeature(new Feature(request.OutputFeatureId)); 
        var created = await _scenarioRepository.AddAsync(scenario, cancellationToken);
        return created.Id;
    }
}


