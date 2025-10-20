using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVAIntegrationModeler.UseCases.Scenarios.Mapping;
using Xunit;

namespace AVAIntegrationModeler.UnitTests.UseCases.Scenarios.Mapping;

public class ScenarioMappingTest
{
  [Fact]
    public void MapScenario_ShouldReturnMappedScenario_WhenInputIsValid()
    {
        // Arrange
        var inputScenario = new Scenario(id: Guid.NewGuid())
            .SetCode("SCEN-001")
            .SetName(new AVAIntegrationModeler.Core.ValueObjects.LocalizedValue { EnglishValue = "Test Scenario" })
            .SetDescription(new AVAIntegrationModeler.Core.ValueObjects.LocalizedValue { EnglishValue = "This is a test Scenario." })
            .SetInputFeature(null)
            .SetOutputFeature(null);

        // Act
        var result = ScenarioMapper.MapToDTO(inputScenario);
        result.ShouldNotBeNull();
        result.Id.ShouldBe(inputScenario.Id);
        result.Code.ShouldBe(inputScenario.Code);
        result.Name.ShouldNotBeNull();
        result.Name.EnglishValue.ShouldBe(inputScenario.Name.EnglishValue);
        result.Description.ShouldNotBeNull();
        result.Description.EnglishValue.ShouldBe(inputScenario.Description.EnglishValue);
        result.InputFeatureId.ShouldBeNull();
        result.OutputFeatureId.ShouldBeNull();
    }

    
}

