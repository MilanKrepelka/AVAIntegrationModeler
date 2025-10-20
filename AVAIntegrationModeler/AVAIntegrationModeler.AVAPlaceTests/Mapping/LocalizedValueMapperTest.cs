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
public class LocalizedValueMapperTest
{
  [Fact]
  public void MapToDTO_Full_Test()
  {
    LocalizedValue<string> name = new LocalizedValue<string>() { Values = new List<LocalizedValueItem<string>>()
      {
        new LocalizedValueItem<string>() { Locale = "cs-CZ", Value = "Název v češtině" },
        new LocalizedValueItem<string>() { Locale = "en-US", Value = "Name in English" },
      } 
    };

    LocalizedValueMapper.MapToDTO(name).CzechValue.ShouldBe("Název v češtině");
    LocalizedValueMapper.MapToDTO(name).EnglishValue.ShouldBe("Name in English");
  }

  [Fact]
  public void MapToDTO_Empty_Test()
  {
    LocalizedValue<string> name = new LocalizedValue<string>()
    {
      Values = new List<LocalizedValueItem<string>>()
      {
      }
    };

    LocalizedValueMapper.MapToDTO(name).CzechValue.ShouldBe(string.Empty);
    LocalizedValueMapper.MapToDTO(name).EnglishValue.ShouldBe(string.Empty);
  }

  [Fact]
  public void MapToDTO_Null_Test()
  {
    LocalizedValue<string> name = new LocalizedValue<string>()
    {
      Values = null
    };

    LocalizedValueMapper.MapToDTO(name).CzechValue.ShouldBe(string.Empty);
    LocalizedValueMapper.MapToDTO(name).EnglishValue.ShouldBe(string.Empty);
  }

  [Fact]
  public void MapToEntity_Full_Test()
  {

    AVAIntegrationModeler.Contracts.DTO.LocalizedValue dto = new AVAIntegrationModeler.Contracts.DTO.LocalizedValue()
    {
      CzechValue = "Název v češtině",
      EnglishValue = "Name in English"
    };

    LocalizedValueMapper.MapToEntity(dto).Values.ShouldNotBeNull();
    LocalizedValueMapper.MapToEntity(dto).Values.ShouldNotBeEmpty();

    LocalizedValueMapper.MapToEntity(dto).Values!.First(item=>item.Locale == "cs-CZ").Value.ShouldBe(dto.CzechValue);
    LocalizedValueMapper.MapToEntity(dto).Values!.First(item => item.Locale == "en-US").Value.ShouldBe(dto.EnglishValue);
  }

  [Fact]
  public void MapToEntity_EmptyText_Test()
  {

    AVAIntegrationModeler.Contracts.DTO.LocalizedValue dto = new AVAIntegrationModeler.Contracts.DTO.LocalizedValue()
    {
      CzechValue = string.Empty,
      EnglishValue = string.Empty,
    };

    LocalizedValueMapper.MapToEntity(dto).Values.ShouldNotBeNull();
    LocalizedValueMapper.MapToEntity(dto).Values.ShouldBeEmpty();
  }
}
