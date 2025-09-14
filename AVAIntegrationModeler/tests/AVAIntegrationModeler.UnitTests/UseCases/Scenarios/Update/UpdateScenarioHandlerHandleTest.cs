using System.Threading;
using Ardalis.SharedKernel;
using AVAIntegrationModeler.Core.ScenarioAggregate;
using AVAIntegrationModeler.Core.ValueObjects;
using AVAIntegrationModeler.UseCases.Scenarios.Create;
using AVAIntegrationModeler.UseCases.Scenarios.Mapping;
using AVAIntegrationModeler.UseCases.Scenarios.Update;
using NSubstitute;
using Xunit;

namespace AVAIntegrationModeler.UnitTests.UseCases.Scenarios.Update;

public class UpdateScenarioHandlerHandleTest
{
  private readonly string _testCode = "test-code";
  private readonly Guid _testId = Guid.NewGuid();
  private readonly LocalizedValue _testName = new() { CzechValue = "Testovací scénář" };
  private readonly LocalizedValue _testDescription = new() { EnglishValue = "Popis scénáře" };
  private readonly IRepository<Scenario> _repository = Substitute.For<IRepository<Scenario>>();
  private readonly UpdateScenarioHandler _handler;

  public UpdateScenarioHandlerHandleTest()
  {
    _handler = new UpdateScenarioHandler(_repository);
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
    var scenario = CreateScenario();
    await _repository.AddAsync(scenario, CancellationToken.None);

    _repository.UpdateAsync(Arg.Any<Scenario>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(1));

    _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult<Scenario?>(scenario));
    
    var newScenario = scenario;
    newScenario.SetCode("NewCode" + Guid.NewGuid());

    var scenarioDTO = ScenarioMapper.MapToDTO(newScenario);
    var command = new UpdateScenarioCommand(scenarioDTO);
    var result = await _handler.Handle(command, CancellationToken.None);
    result.ShouldNotBeNull();
    result.IsSuccess.ShouldBeTrue();

  }
}
