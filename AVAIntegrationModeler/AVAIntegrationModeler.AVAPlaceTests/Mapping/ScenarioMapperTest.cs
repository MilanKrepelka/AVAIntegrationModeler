using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASOL.Core.Localization;
using ASOL.DataService.Contracts;
using AVAIntegrationModeler.AVAPlace.Mapping;
using AVAIntegrationModeler.Contracts.DTO;
using Shouldly;

namespace AVAIntegrationModeler.AVAPlaceTests.Mapping;
public class ScenarioMapperTest
{
  [Fact]
  public void Map_IntegrationScenarioDefinition_To_DTO_Test()
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

  [Fact]
  public void Map_ScenarioDTO_ToIntegrationScenarioDefinition_Test()
  {
    ScenarioDTO scenarioDTO = new ScenarioDTO
    {
      Code = "Scenario",
      Id = Guid.NewGuid(),
      Name = new LocalizedValue() { CzechValue = "Scénář", EnglishValue = "Scenario" },
      Description = new LocalizedValue() { CzechValue = "Popis scénáře", EnglishValue = "Scenario description" },
      InputFeatureId = Guid.NewGuid(),
      OutputFeatureId = Guid.NewGuid(),
    };
    
    ScenarioMapper.MapToEntity(scenarioDTO).Id.ShouldBe(scenarioDTO.Id.ToString());
    ScenarioMapper.MapToEntity(scenarioDTO).InputFeatureCodeOrId.ShouldBe(scenarioDTO.InputFeatureId.ToString());
    ScenarioMapper.MapToEntity(scenarioDTO).OutputFeatureCodeOrId.ShouldBe(scenarioDTO.OutputFeatureId.ToString());
    ScenarioMapper.MapToEntity(scenarioDTO).Code.ShouldBe(scenarioDTO.Code);
    ScenarioMapper.MapToEntity(scenarioDTO).Name.Values!.ShouldContain(x => x.Locale == "cs-CZ" && x.Value == "Scénář");
    ScenarioMapper.MapToEntity(scenarioDTO).Name.Values!.ShouldContain(x => x.Locale == "en-US" && x.Value == "Scenario");
    ScenarioMapper.MapToEntity(scenarioDTO).Description.Values!.ShouldContain(x => x.Locale == "cs-CZ" && x.Value == "Popis scénáře");
    ScenarioMapper.MapToEntity(scenarioDTO).Description.Values!.ShouldContain(x => x.Locale == "en-US" && x.Value == "Scenario description");
  }


  [Fact]
  public void Map_IntegrationScenarioSummary_To_DTO_Test()
  {
    IntegrationScenarioSummary integrationScenarioSummary = new IntegrationScenarioSummary
    {
      Code = "Scenario",
      Id = Guid.NewGuid().ToString(),
      Name = new LocalizedValue<string>() { Values = new LocalizedValueItem<string>[] { new() { Locale = "cs-CZ", Value = "Scénář" }, new() { Locale = "en-US", Value = "Scenario" } } },
      Description = new LocalizedValue<string>() { Values = new LocalizedValueItem<string>[] { new() { Locale = "cs-CZ", Value = "Popis scénáře" }, new() { Locale = "en-US", Value = "Scenario description" } } },
      InputFeatureId = Guid.NewGuid().ToString(),
      OutputFeatureId = Guid.NewGuid().ToString(),
    };

    ScenarioMapper.MapToDTO(integrationScenarioSummary).Id.ShouldBe(Guid.Parse(integrationScenarioSummary.Id));
    ScenarioMapper.MapToDTO(integrationScenarioSummary).Code.ShouldBe(integrationScenarioSummary.Code);
    ScenarioMapper.MapToDTO(integrationScenarioSummary).InputFeatureId.ShouldBe(Guid.Parse(integrationScenarioSummary.InputFeatureId));
    ScenarioMapper.MapToDTO(integrationScenarioSummary).OutputFeatureId.ShouldBe(Guid.Parse(integrationScenarioSummary.OutputFeatureId));

    ScenarioMapper.MapToDTO(integrationScenarioSummary).Name.CzechValue.ShouldBe("Scénář");
    ScenarioMapper.MapToDTO(integrationScenarioSummary).Name.EnglishValue.ShouldBe("Scenario");
    ScenarioMapper.MapToDTO(integrationScenarioSummary).Description.CzechValue.ShouldBe("Popis scénáře");
    ScenarioMapper.MapToDTO(integrationScenarioSummary).Description.EnglishValue.ShouldBe("Scenario description");
  }

  [Fact]
  public void Map_DTO_IntegrationScenarioSummary_Test()
  {
    ScenarioDTO scenarioDTO = new ScenarioDTO
    {
      Code = "Scenario",
      Id = Guid.NewGuid(),
      Name = new LocalizedValue() { CzechValue = "Scénář", EnglishValue = "Scenario" },
      Description = new LocalizedValue() { CzechValue = "Popis scénáře", EnglishValue = "Scenario description" },
      InputFeatureId = Guid.NewGuid(),
      OutputFeatureId = Guid.NewGuid(),
    };

    ScenarioMapper.MapToIntegrationScenarioSummary(scenarioDTO).Id.ShouldBe(scenarioDTO.Id.ToString());

    /*
    ScenarioMapper.MapToDTO(integrationScenarioSummary).Code.ShouldBe(integrationScenarioSummary.Code);
    ScenarioMapper.MapToDTO(integrationScenarioSummary).InputFeatureId.ShouldBe(Guid.Parse(integrationScenarioSummary.InputFeatureId));
    ScenarioMapper.MapToDTO(integrationScenarioSummary).OutputFeatureId.ShouldBe(Guid.Parse(integrationScenarioSummary.OutputFeatureId));

    ScenarioMapper.MapToDTO(integrationScenarioSummary).Name.CzechValue.ShouldBe("Scénář");
    ScenarioMapper.MapToDTO(integrationScenarioSummary).Name.EnglishValue.ShouldBe("Scenario");
    ScenarioMapper.MapToDTO(integrationScenarioSummary).Description.CzechValue.ShouldBe("Popis scénáře");
    ScenarioMapper.MapToDTO(integrationScenarioSummary).Description.EnglishValue.ShouldBe("Scenario description");
    */
  }

  
}
