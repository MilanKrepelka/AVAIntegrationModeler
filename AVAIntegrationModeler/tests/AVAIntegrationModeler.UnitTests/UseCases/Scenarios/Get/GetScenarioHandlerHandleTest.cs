using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVAIntegrationModeler.Core.ScenarioAggregate.Specifications;
using AVAIntegrationModeler.Core.ValueObjects;
using AVAIntegrationModeler.UseCases.Scenarios.Create;
using AVAIntegrationModeler.UseCases.Scenarios.Get;

namespace AVAIntegrationModeler.UnitTests.UseCases.Scenarios.Get;


public class GetScenarioHandlerHandleTest
{
  private readonly string _testCode = "test-code";
  private readonly Guid _testId = Guid.NewGuid();
  private readonly LocalizedValue _testName = new LocalizedValue { CzechValue = "Testovací scénář" };
  private readonly LocalizedValue _testDescription = new LocalizedValue { EnglishValue = "Popis scénáře" };
  private readonly IReadRepository<Scenario> _repository = Substitute.For<IReadRepository<Scenario>>();
  private GetScenarioHandler _handler;

  public GetScenarioHandlerHandleTest()
  {
    _handler = new GetScenarioHandler(_repository);
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

    _repository.FirstOrDefaultAsync(Arg.Any<ScenarioByIdSpec>(), Arg.Any<CancellationToken>())
          .Returns(Task.FromResult<Scenario?>(CreateScenario()));
    var command = new GetScenarioQuery(_testId);
    var result = await _handler.Handle(command, CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
    result.Value.Id.ShouldBe(_testId);
  }
}
