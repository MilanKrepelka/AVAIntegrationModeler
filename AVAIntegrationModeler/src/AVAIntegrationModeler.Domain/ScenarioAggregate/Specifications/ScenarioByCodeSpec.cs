using AVAIntegrationModeler.Domain.ScenarioAggregate;

namespace AVAIntegrationModeler.Domain.ScenarioAggregate.Specifications;

public class ScenarioByCodeSpec : Specification<Scenario>
{
  public ScenarioByCodeSpec(string scenarioCode) =>
    Query
        .Where(scenario => scenario.Code == scenarioCode);
}
