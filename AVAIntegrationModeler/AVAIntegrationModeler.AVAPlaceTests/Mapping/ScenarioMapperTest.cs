using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASOL.Core.Localization;
using ASOL.DataService.Contracts;
using AVAIntegrationModeler.AVAPlace.Mapping;
using Shouldly;

namespace AVAIntegrationModeler.AVAPlaceTests.Mapping;
public class ScenarioMapperTest
{
  [Fact]
  public void Full_MapToDTO_Test()
  {
    IntegrationScenarioDefinition integrationScenarioDefinition = new IntegrationScenarioDefinition
    {
      Code = "Scenario",
      Id = Guid.NewGuid().ToString(),
      Name = new LocalizedValue<string>() { Values = new LocalizedValueItem<string>[] { new() { Locale = "cs-CZ", Value = "Scénář" }, new() { Locale = "en-US", Value = "Scenario" } } },
      Description = new LocalizedValue<string>() { Values = new LocalizedValueItem<string>[] { new() { Locale = "cs-CZ", Value = "Popis scénáře" }, new() { Locale = "en-US", Value = "Scenario description" } } },
      InputFeatureCodeOrId = Guid.NewGuid().ToString(),
      OutputFeatureCodeOrId = Guid.NewGuid().ToString()
    };


    ScenarioMapper.MapToDTO(integrationScenarioDefinition).Id.ShouldBe(Guid.Parse(integrationScenarioDefinition.Id));
    ScenarioMapper.MapToDTO(integrationScenarioDefinition).Code.ShouldBe(integrationScenarioDefinition.Code);
    ScenarioMapper.MapToDTO(integrationScenarioDefinition).InputFeatureId.ShouldBe(Guid.Parse(integrationScenarioDefinition.InputFeatureCodeOrId));
    ScenarioMapper.MapToDTO(integrationScenarioDefinition).OutputFeatureId.ShouldBe(Guid.Parse(integrationScenarioDefinition.OutputFeatureCodeOrId));

    ScenarioMapper.MapToDTO(integrationScenarioDefinition).Name.CzechValue.ShouldNotBeNullOrEmpty();
    ScenarioMapper.MapToDTO(integrationScenarioDefinition).Name.EnglishValue.ShouldNotBeNullOrEmpty();
    ScenarioMapper.MapToDTO(integrationScenarioDefinition).Description.CzechValue.ShouldNotBeNullOrEmpty();
    ScenarioMapper.MapToDTO(integrationScenarioDefinition).Description.EnglishValue.ShouldNotBeNullOrEmpty();


  }
}
