using AVAIntegrationModeler.Domain.ScenarioAggregate;

namespace AVAIntegrationModeler.Domain.ScenarioAggregate.Specifications;

public class ScenarioByIdSpec : Specification<Scenario>
{
  public ScenarioByIdSpec(Guid scenarioId) =>
    Query
        .Where(scenario => scenario.Id == scenarioId);
}
