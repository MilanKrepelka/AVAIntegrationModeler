using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.ScenarioAggregate;
using AVAIntegrationModeler.Infrastructure.ValidationServices;
using AVAIntegrationModeler.UseCases.Scenarios;
using NSubstitute;
using Shouldly;

namespace AVAIntegrationModeler.Infrastructure.Test.ValidationServices;

/// <summary>
/// Testy pro ScenarioValidationService — ověřují unikátnost kódu scénáře při vytvoření
/// (ValidateForCreate) i při aktualizaci (Validate).
/// </summary>
public class ScenarioValidationServiceTests
{
  private readonly IScenariosQueryService _queryService = Substitute.For<IScenariosQueryService>();
  private readonly ScenarioValidationService _sut;

  public ScenarioValidationServiceTests()
  {
    _sut = new ScenarioValidationService(_queryService);
  }

  private static Scenario NewScenario(Guid id, string code) => new Scenario(id).SetCode(code);

  // ---------------------------------------------------------------
  // ValidateForCreate
  // ---------------------------------------------------------------

  [Fact]
  public async Task ValidateForCreate_UniqueCodeAndId_ReturnsSuccess()
  {
    _queryService.ExistsByCodeAsync(Datasource.Database, "SC-1", Arg.Any<CancellationToken>()).Returns(false);
    _queryService.ExistsByIdAsync(Datasource.Database, Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

    var scenario = NewScenario(Guid.NewGuid(), "SC-1");

    var result = await _sut.ValidateForCreate(Datasource.Database, scenario, CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
  }

  [Fact]
  public async Task ValidateForCreate_DuplicateCode_ReturnsInvalid()
  {
    _queryService.ExistsByCodeAsync(Datasource.Database, "SC-1", Arg.Any<CancellationToken>()).Returns(true);
    _queryService.ExistsByIdAsync(Datasource.Database, Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

    var scenario = NewScenario(Guid.NewGuid(), "SC-1");

    var result = await _sut.ValidateForCreate(Datasource.Database, scenario, CancellationToken.None);

    result.IsSuccess.ShouldBeFalse();
    result.Status.ShouldBe(Ardalis.Result.ResultStatus.Invalid);
  }

  [Fact]
  public async Task ValidateForCreate_DuplicateId_ReturnsInvalid()
  {
    _queryService.ExistsByCodeAsync(Datasource.Database, "SC-1", Arg.Any<CancellationToken>()).Returns(false);
    _queryService.ExistsByIdAsync(Datasource.Database, Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);

    var scenario = NewScenario(Guid.NewGuid(), "SC-1");

    var result = await _sut.ValidateForCreate(Datasource.Database, scenario, CancellationToken.None);

    result.IsSuccess.ShouldBeFalse();
    result.Status.ShouldBe(Ardalis.Result.ResultStatus.Invalid);
  }

  // ---------------------------------------------------------------
  // Validate (update)
  // ---------------------------------------------------------------

  [Fact]
  public async Task Validate_CodeNotUsedByAnyScenario_ReturnsSuccess()
  {
    _queryService.ExistsByCodeAsync(Datasource.Database, "SC-NEW", Arg.Any<CancellationToken>()).Returns(false);

    var scenario = NewScenario(Guid.NewGuid(), "SC-NEW");

    var result = await _sut.Validate(Datasource.Database, scenario, CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
  }

  [Fact]
  public async Task Validate_CodeBelongsToTheSameScenario_ReturnsSuccess()
  {
    var scenarioId = Guid.NewGuid();
    _queryService.ExistsByCodeAsync(Datasource.Database, "SC-1", Arg.Any<CancellationToken>()).Returns(true);
    _queryService.GetScenario(Datasource.Database, "SC-1", Arg.Any<CancellationToken>())
      .Returns(new ScenarioDTO { Id = scenarioId, Code = "SC-1" });

    var scenario = NewScenario(scenarioId, "SC-1");

    var result = await _sut.Validate(Datasource.Database, scenario, CancellationToken.None);

    result.IsSuccess.ShouldBeTrue();
  }

  [Fact]
  public async Task Validate_CodeBelongsToAnotherScenario_ReturnsInvalid()
  {
    var otherScenarioId = Guid.NewGuid();
    _queryService.ExistsByCodeAsync(Datasource.Database, "SC-1", Arg.Any<CancellationToken>()).Returns(true);
    _queryService.GetScenario(Datasource.Database, "SC-1", Arg.Any<CancellationToken>())
      .Returns(new ScenarioDTO { Id = otherScenarioId, Code = "SC-1" });

    var scenario = NewScenario(Guid.NewGuid(), "SC-1");

    var result = await _sut.Validate(Datasource.Database, scenario, CancellationToken.None);

    result.IsSuccess.ShouldBeFalse();
    result.Status.ShouldBe(Ardalis.Result.ResultStatus.Invalid);
  }
}
