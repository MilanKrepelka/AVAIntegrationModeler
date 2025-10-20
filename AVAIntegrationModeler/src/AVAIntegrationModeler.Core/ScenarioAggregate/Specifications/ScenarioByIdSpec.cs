using AVAIntegrationModeler.Core.ScenarioAggregate;

namespace AVAIntegrationModeler.Core.ScenarioAggregate.Specifications;

public class ScenarioByIdSpec : Specification<Scenario>
{
  public ScenarioByIdSpec(Guid scenarioId) =>
    Query
        .Where(scenario => scenario.Id == scenarioId);
}
