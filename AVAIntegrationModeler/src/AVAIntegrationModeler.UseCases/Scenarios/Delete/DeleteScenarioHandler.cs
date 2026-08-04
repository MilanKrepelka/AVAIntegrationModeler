using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.ScenarioAggregate;

namespace AVAIntegrationModeler.UseCases.Scenarios.Delete;

public class DeleteScenarioHandler(IRepository<Scenario> repository, IScenariosQueryService scenariosQueryService)
  : ICommandHandler<DeleteScenarioCommand, Result>
{
  public async Task<Result> Handle(DeleteScenarioCommand request, CancellationToken cancellationToken)
  {
    var aggregateToDelete = await repository.FirstOrDefaultAsync(
      new Domain.ScenarioAggregate.Specifications.ScenarioByCodeSpec(request.ScenarioCode), cancellationToken);
    if (aggregateToDelete == null) return Result.NotFound();

    await repository.DeleteAsync(aggregateToDelete, cancellationToken);
    scenariosQueryService.InvalidateCache(Datasource.Database);
    return Result.Success();
  }
}
