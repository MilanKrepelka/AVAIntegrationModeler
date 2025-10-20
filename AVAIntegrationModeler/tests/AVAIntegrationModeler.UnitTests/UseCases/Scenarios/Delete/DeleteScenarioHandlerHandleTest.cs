using Ardalis.Result;
using Ardalis.SharedKernel;
using AVAIntegrationModeler.Core.ScenarioAggregate;
using AVAIntegrationModeler.Core.ValueObjects;
using AVAIntegrationModeler.UseCases.Scenarios.Delete;
using NSubstitute;
using Xunit;

namespace AVAIntegrationModeler.UnitTests.UseCases.Scenarios.Delete;

public class DeleteScenarioHandlerHandleTest
{
  private readonly string _testCode = "test-code";
  private readonly Guid _testId = Guid.NewGuid();
  private readonly LocalizedValue _testName = new() { CzechValue = "Testovací scénář" };
  private readonly LocalizedValue _testDescription = new() { EnglishValue = "Popis scénáře" };
  private readonly IRepository<Scenario> _repository = Substitute.For<IRepository<Scenario>>();
  private readonly DeleteScenarioHandler _handler;

  public DeleteScenarioHandlerHandleTest()
  {
    _handler = new DeleteScenarioHandler(_repository);
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
  public async Task ReturnsNotFound()
  {
    Scenario scenario = CreateScenario();
    var deleteCommandCommand = new DeleteScenarioCommand(scenario.Id);
    var deleteResult = await _handler.Handle(deleteCommandCommand, CancellationToken.None);

    deleteResult.ShouldNotBeNull();
    deleteResult.Status.ShouldBe(ResultStatus.NotFound);

  }
}
