namespace AVAIntegrationModeler.UnitTests.Core.ScenarioAggregate;

public class ScenarioConstructor
{
  private readonly string _testCode = "TestCode";
  private readonly Guid _testId = Guid.NewGuid();
  private Scenario? _testedScenario;

  private Scenario CreateScenario()
  {
    return new Scenario(_testId);
  }

  [Fact]
  public void InitializesCode()
  {
    _testedScenario = CreateScenario();
    _testedScenario.Id.ShouldBe(_testId);
    _testedScenario.Code.ShouldBe(_testCode);
  }
}
