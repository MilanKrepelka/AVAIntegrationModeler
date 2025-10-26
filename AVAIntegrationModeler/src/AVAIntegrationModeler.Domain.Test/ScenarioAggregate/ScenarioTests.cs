using AVAIntegrationModeler.Domain.ScenarioAggregate;
using AVAIntegrationModeler.Domain.ValueObjects;
using Shouldly;

namespace AVAIntegrationModeler.Domain.Test.ScenarioAggregate;

/// <summary>
/// Unit testy pro agregát Scenario.
/// </summary>
public class ScenarioTests
{
  [Fact]
  public void Constructor_ShouldSetId()
  {
    // Arrange
    var id = Guid.NewGuid();

    // Act
    var scenario = new Scenario(id);

    // Assert
    scenario.Id.ShouldBe(id);
  }

  [Fact]
  public void SetCode_ShouldUpdateCode()
  {
    // Arrange
    var scenario = new Scenario(Guid.NewGuid());
    var code = "SCEN-001";

    // Act
    scenario.SetCode(code);

    // Assert
    scenario.Code.ShouldBe(code);
  }

  [Fact]
  public void SetCode_WithNullOrEmpty_ShouldThrowException()
  {
    // Arrange
    var scenario = new Scenario(Guid.NewGuid());

    // Act & Assert
    Should.Throw<ArgumentException>(() => scenario.SetCode(null!));
    Should.Throw<ArgumentException>(() => scenario.SetCode(string.Empty));
  }

  [Fact]
  public void SetName_ShouldUpdateName()
  {
    // Arrange
    var scenario = new Scenario(Guid.NewGuid());
    var name = new LocalizedValue { CzechValue = "Název", EnglishValue = "Name" };

    // Act
    scenario.SetName(name);

    // Assert
    scenario.Name.ShouldBe(name);
  }

  [Fact]
  public void SetDescription_ShouldUpdateDescription()
  {
    // Arrange
    var scenario = new Scenario(Guid.NewGuid());
    var description = new LocalizedValue { CzechValue = "Popis", EnglishValue = "Description" };

    // Act
    scenario.SetDescription(description);

    // Assert
    scenario.Description.ShouldBe(description);
  }

  [Fact]
  public void SetInputFeature_ShouldUpdateInputFeature()
  {
    // Arrange
    var scenario = new Scenario(Guid.NewGuid());
    var featureId = Guid.NewGuid();

    // Act
    scenario.SetInputFeature(featureId);

    // Assert
    scenario.InputFeature.ShouldBe(featureId);
  }

  [Fact]
  public void SetOutputFeature_ShouldUpdateOutputFeature()
  {
    // Arrange
    var scenario = new Scenario(Guid.NewGuid());
    var featureId = Guid.NewGuid();

    // Act
    scenario.SetOutputFeature(featureId);

    // Assert
    scenario.OutputFeature.ShouldBe(featureId);
  }

  [Fact]
  public void FluentAPI_ShouldChainMethodCalls()
  {
    // Arrange
    var id = Guid.NewGuid();
    var inputFeatureId = Guid.NewGuid();
    var outputFeatureId = Guid.NewGuid();

    // Act
    var scenario = new Scenario(id)
      .SetCode("SCEN-001")
      .SetName(new LocalizedValue { CzechValue = "Test", EnglishValue = "Test" })
      .SetDescription(new LocalizedValue { CzechValue = "Popis", EnglishValue = "Description" })
      .SetInputFeature(inputFeatureId)
      .SetOutputFeature(outputFeatureId);

    // Assert
    scenario.Id.ShouldBe(id);
    scenario.Code.ShouldBe("SCEN-001");
    scenario.InputFeature.ShouldBe(inputFeatureId);
    scenario.OutputFeature.ShouldBe(outputFeatureId);
  }
}
