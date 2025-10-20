using AVAIntegrationModeler.Core.Interfaces;
using AVAIntegrationModeler.Core.ScenarioAggregate;

namespace AVAIntegrationModeler.UseCases.Scenarios.Delete;

public class DeleteScenarioHandler(IRepository<Scenario> repository) : ICommandHandler<DeleteScenarioCommand, Result>
{
  private readonly IRepository<Scenario> _repository = repository;

  public async Task<Result> Handle(DeleteScenarioCommand request, CancellationToken cancellationToken) 
    {
    // This Approach: Keep Domain Events in the Domain Model / Core project; this becomes a pass-through
    // This is @ardalis's preferred approach
    
    // Another Approach: Do the real work here including dispatching domain events - change the event from internal to public
    // @ardalis prefers using the service above so that **domain** event behavior remains in the **domain model** (core project)
     var aggregateToDelete = await _repository.GetByIdAsync(request.ScenarioId, cancellationToken);
    if (aggregateToDelete == null) return Result.NotFound();

    await _repository.DeleteAsync(aggregateToDelete, cancellationToken);
    return Result.Success();
    
    // if (aggregateToDelete == null) return Result.NotFound();
    // await _repository.DeleteAsync(aggregateToDelete);
    // var domainEvent = new ContributorDeletedEvent(request.ContributorId);
    // await _mediator.Publish(domainEvent);// return Result.Success();
  }
}
