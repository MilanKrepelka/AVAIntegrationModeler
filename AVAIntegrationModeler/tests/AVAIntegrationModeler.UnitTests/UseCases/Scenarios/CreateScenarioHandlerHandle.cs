using AVAIntegrationModeler.Core.ScenarioAggregate;
using AVAIntegrationModeler.Core.ValueObjects;
using AVAIntegrationModeler.UseCases.Scenarios.Create;
using Ardalis.SharedKernel;
using NSubstitute;
using Xunit;

namespace AVAIntegrationModeler.UnitTests.UseCases.Scenarios;

public class CreateScenarioHandlerHandle
{
  private readonly string _testCode = "test-code";
  private readonly LocalizedValue _testName = new LocalizedValue { CzechValue = "Testovací scénář" };
  private readonly LocalizedValue _testDescription = new LocalizedValue { EnglishValue = "Popis scénáře" };
  private readonly IRepository<Scenario> _repository = Substitute.For<IRepository<Scenario>>();
  private CreateScenarioHandler _handler;

  public CreateScenarioHandlerHandle()
  {
    _handler = new CreateScenarioHandler(_repository);
  }

  private Scenario CreateScenario()
  {
    return new Scenario(_testCode)
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
    var command = new CreateScenarioCommand(_testCode, _testName, _testDescription, null, null);
    var result = await _handler.Handle(command, CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
  }
}
