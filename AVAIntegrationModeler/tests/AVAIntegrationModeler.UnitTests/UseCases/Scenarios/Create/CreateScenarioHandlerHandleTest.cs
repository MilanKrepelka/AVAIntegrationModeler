using AVAIntegrationModeler.Core.ScenarioAggregate;
using AVAIntegrationModeler.Core.ValueObjects;
using AVAIntegrationModeler.UseCases.Scenarios.Create;
using Ardalis.SharedKernel;
using NSubstitute;
using Xunit;

namespace AVAIntegrationModeler.UnitTests.UseCases.Scenarios.Create;

public class CreateScenarioHandlerHandleTest
{
  private readonly string _testCode = "test-code";
  private readonly Guid _testId = Guid.NewGuid();
  private readonly LocalizedValue _testName = new LocalizedValue { CzechValue = "Testovací scénář" };
  private readonly LocalizedValue _testDescription = new LocalizedValue { EnglishValue = "Popis scénáře" };
  private readonly IRepository<Scenario> _repository = Substitute.For<IRepository<Scenario>>();
  private CreateScenarioHandler _handler;

  public CreateScenarioHandlerHandleTest()
  {
    _handler = new CreateScenarioHandler(_repository);
  }

  private Scenario CreateScenario()
  {
    return new Scenario(_testId)
      .SetCode(_testCode)
      .SetName(_testName)
      .SetDescription(_testDescription)
      .SetInputFeature(null)
      .SetOutputFeature(null);
  }

  [Fact]
    public async Task ReturnsSuccessGivenValidScenario()
    {
        _repository.AddAsync(Arg.Any<Scenario>(), Arg.Any<CancellationToken>())
          .Returns(Task.FromResult(CreateScenario()));
        var command = new CreateScenarioCommand(_testId, _testCode, _testName, _testDescription, null, null);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(_testId);
    }
}
