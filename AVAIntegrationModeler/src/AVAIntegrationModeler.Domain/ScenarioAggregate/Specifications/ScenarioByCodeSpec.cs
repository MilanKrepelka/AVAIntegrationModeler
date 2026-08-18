using AVAIntegrationModeler.Domain.ScenarioAggregate;

namespace AVAIntegrationModeler.Domain.ScenarioAggregate.Specifications;

/// <summary>
/// Specifikace pro vyhledání scénáře podle kódu. Porovnání kódu je case-insensitive.
/// </summary>
public class ScenarioByCodeSpec : Specification<Scenario>
{
  public ScenarioByCodeSpec(string scenarioCode) =>
    Query
        .Where(scenario => scenario.Code.ToUpper() == scenarioCode.ToUpper());
}
