namespace AVAIntegrationModeler.UnitTests.Core.ScenarioAggregate;

public class ScenarioConstructor
{
  private readonly string _testCode = "TestCode";
  private Scenario? _testedScenario;

  private Scenario CreateScenario()
  {
    return new Scenario(_testCode);
  }

  [Fact]
  public void InitializesCode()
  {
    _testedScenario = CreateScenario();

    _testedScenario.Code.ShouldBe(_testCode);
  }
}
