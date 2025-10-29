using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace AVAIntegrationModeler.UseCases.Test.Scenarios.Mapping;

/// <summary>
/// Unit testy pro LocalizedValueMapper.
/// </summary>
public class LocalizedValueMapperTests
{
  #region MapToDTO Tests

  [Fact]
  public void MapToDTO_ShouldReturnMappedDTO_WhenDomainEntityIsValid()
  {
    // Arrange
    var domainEntity = new Domain.ValueObjects.LocalizedValue
    {
      CzechValue = "Název v češtině",
      EnglishValue = "Name in English"
    };

    // Act
    var result = UseCases.LocalizedValueMapper.MapToDTO(domainEntity);

    // Assert
    result.ShouldNotBeNull();
    result.CzechValue.ShouldBe("Název v češtině");
    result.EnglishValue.ShouldBe("Name in English");
  }

  [Fact]
  public void MapToDTO_ShouldReturnEmptyStrings_WhenDomainEntityIsNull()
  {
    // Arrange
    Domain.ValueObjects.LocalizedValue? domainEntity = null;

    // Act
    var result = UseCases.LocalizedValueMapper.MapToDTO(domainEntity!);

    // Assert
    result.ShouldNotBeNull();
    result.CzechValue.ShouldBe(string.Empty);
    result.EnglishValue.ShouldBe(string.Empty);
  }

  [Fact]
  public void MapToDTO_ShouldHandleNullCzechValue()
  {
    // Arrange
    var domainEntity = new Domain.ValueObjects.LocalizedValue
    {
      CzechValue = null!,
      EnglishValue = "English value"
    };

    // Act
    var result = UseCases.LocalizedValueMapper.MapToDTO(domainEntity);

    // Assert
    result.ShouldNotBeNull();
    result.CzechValue.ShouldBe(string.Empty);
    result.EnglishValue.ShouldBe("English value");
  }

  [Fact]
  public void MapToDTO_ShouldHandleNullEnglishValue()
  {
    // Arrange
    var domainEntity = new Domain.ValueObjects.LocalizedValue
    {
      CzechValue = "České hodnota",
      EnglishValue = null!
    };

    // Act
    var result = UseCases.LocalizedValueMapper.MapToDTO(domainEntity);

    // Assert
    result.ShouldNotBeNull();
    result.CzechValue.ShouldBe("České hodnota");
    result.EnglishValue.ShouldBe(string.Empty);
  }

  [Fact]
  public void MapToDTO_ShouldHandleBothNullValues()
  {
    // Arrange
    var domainEntity = new Domain.ValueObjects.LocalizedValue
    {
      CzechValue = null!,
      EnglishValue = null!
    };

    // Act
    var result = UseCases.LocalizedValueMapper.MapToDTO(domainEntity);

    // Assert
    result.ShouldNotBeNull();
    result.CzechValue.ShouldBe(string.Empty);
    result.EnglishValue.ShouldBe(string.Empty);
  }

  [Fact]
  public void MapToDTO_ShouldHandleEmptyStrings()
  {
    // Arrange
    var domainEntity = new Domain.ValueObjects.LocalizedValue
    {
      CzechValue = string.Empty,
      EnglishValue = string.Empty
    };

    // Act
    var result = UseCases.LocalizedValueMapper.MapToDTO(domainEntity);

    // Assert
    result.ShouldNotBeNull();
    result.CzechValue.ShouldBe(string.Empty);
    result.EnglishValue.ShouldBe(string.Empty);
  }

  [Fact]
  public void MapToDTO_ShouldHandleSpecialCharacters()
  {
    // Arrange
    var domainEntity = new Domain.ValueObjects.LocalizedValue
    {
      CzechValue = "Úplný popis s českými znaky: ěščřžýáíé",
      EnglishValue = "Complete description with special chars: @#$%"
    };

    // Act
    var result = UseCases.LocalizedValueMapper.MapToDTO(domainEntity);

    // Assert
    result.ShouldNotBeNull();
    result.CzechValue.ShouldBe("Úplný popis s českými znaky: ěščřžýáíé");
    result.EnglishValue.ShouldBe("Complete description with special chars: @#$%");
  }

  [Fact]
  public void MapToDTO_ShouldHandleLongStrings()
  {
    // Arrange
    var longCzechText = new string('Č', 1000);
    var longEnglishText = new string('E', 1000);
    var domainEntity = new Domain.ValueObjects.LocalizedValue
    {
      CzechValue = longCzechText,
      EnglishValue = longEnglishText
    };

    // Act
    var result = UseCases.LocalizedValueMapper.MapToDTO(domainEntity);

    // Assert
    result.ShouldNotBeNull();
    result.CzechValue.ShouldBe(longCzechText);
    result.EnglishValue.ShouldBe(longEnglishText);
  }

  #endregion

  #region MapToEntity Tests

  [Fact]
  public void MapToEntity_ShouldReturnMappedEntity_WhenDTOIsValid()
  {
    // Arrange
    var dto = new Contracts.DTO.LocalizedValue
    {
      CzechValue = "Název v češtině",
      EnglishValue = "Name in English"
    };

    // Act
    var result = UseCases.LocalizedValueMapper.MapToEntity(dto);

    // Assert
    result.ShouldNotBeNull();
    result.CzechValue.ShouldBe("Název v češtině");
    result.EnglishValue.ShouldBe("Name in English");
  }

  [Fact]
  public void MapToEntity_ShouldHandleNullDTO()
  {
    // Arrange
    Contracts.DTO.LocalizedValue? dto = null;

    // Act
    var result = UseCases.LocalizedValueMapper.MapToEntity(dto!);

    // Assert
    result.ShouldNotBeNull();
    // Doménový objekt by měl být vytvořen s výchozími hodnotami
  }

  [Fact]
  public void MapToEntity_ShouldHandleEmptyStrings()
  {
    // Arrange
    var dto = new Contracts.DTO.LocalizedValue
    {
      CzechValue = string.Empty,
      EnglishValue = string.Empty
    };

    // Act
    var result = UseCases.LocalizedValueMapper.MapToEntity(dto);

    // Assert
    result.ShouldNotBeNull();
    result.CzechValue.ShouldBe(string.Empty);
    result.EnglishValue.ShouldBe(string.Empty);
  }

  [Fact]
  public void MapToEntity_ShouldHandleNullValues()
  {
    // Arrange
    var dto = new Contracts.DTO.LocalizedValue
    {
      CzechValue = null!,
      EnglishValue = null!
    };

    // Act
    var result = UseCases.LocalizedValueMapper.MapToEntity(dto);

    // Assert
    result.ShouldNotBeNull();
  }

  [Fact]
  public void MapToEntity_ShouldHandleOnlyCzechValue()
  {
    // Arrange
    var dto = new Contracts.DTO.LocalizedValue
    {
      CzechValue = "Pouze česky",
      EnglishValue = null!
    };

    // Act
    var result = UseCases.LocalizedValueMapper.MapToEntity(dto);

    // Assert
    result.ShouldNotBeNull();
    result.CzechValue.ShouldBe("Pouze česky");
  }

  [Fact]
  public void MapToEntity_ShouldHandleOnlyEnglishValue()
  {
    // Arrange
    var dto = new Contracts.DTO.LocalizedValue
    {
      CzechValue = null!,
      EnglishValue = "Only English"
    };

    // Act
    var result = UseCases.LocalizedValueMapper.MapToEntity(dto);

    // Assert
    result.ShouldNotBeNull();
    result.EnglishValue.ShouldBe("Only English");
  }

  #endregion

  #region Roundtrip Tests

  [Fact]
  public void MapToDTO_AndBack_ShouldPreserveValues()
  {
    // Arrange
    var originalEntity = new Domain.ValueObjects.LocalizedValue
    {
      CzechValue = "Kompletní feature",
      EnglishValue = "Complete Feature"
    };

    // Act
    var dto = UseCases.LocalizedValueMapper.MapToDTO(originalEntity);
    var mappedBackEntity = UseCases.LocalizedValueMapper.MapToEntity(dto);

    // Assert
    mappedBackEntity.ShouldNotBeNull();
    mappedBackEntity.CzechValue.ShouldBe(originalEntity.CzechValue);
    mappedBackEntity.EnglishValue.ShouldBe(originalEntity.EnglishValue);
  }

  [Fact]
  public void MapToEntity_AndBack_ShouldPreserveValues()
  {
    // Arrange
    var originalDto = new Contracts.DTO.LocalizedValue
    {
      CzechValue = "Úplný popis s českými znaky",
      EnglishValue = "Complete description with special chars"
    };

    // Act
    var entity = UseCases.LocalizedValueMapper.MapToEntity(originalDto);
    var mappedBackDto = UseCases.LocalizedValueMapper.MapToDTO(entity);

    // Assert
    mappedBackDto.ShouldNotBeNull();
    mappedBackDto.CzechValue.ShouldBe(originalDto.CzechValue);
    mappedBackDto.EnglishValue.ShouldBe(originalDto.EnglishValue);
  }

  [Fact]
  public void MapToDTO_AndBack_ShouldHandleNullValues()
  {
    // Arrange
    var originalEntity = new Domain.ValueObjects.LocalizedValue
    {
      CzechValue = null!,
      EnglishValue = null!
    };

    // Act
    var dto = UseCases.LocalizedValueMapper.MapToDTO(originalEntity);
    var mappedBackEntity = UseCases.LocalizedValueMapper.MapToEntity(dto);

    // Assert
    mappedBackEntity.ShouldNotBeNull();
    // Po roundtripu by měly být prázdné řetězce místo null
    mappedBackEntity.CzechValue.ShouldBe(string.Empty);
    mappedBackEntity.EnglishValue.ShouldBe(string.Empty);
  }

  #endregion

  #region Equality Tests

  [Fact]
  public void MapToDTO_ShouldCreateEqualObjects_ForSameInput()
  {
    // Arrange
    var domainEntity = new Domain.ValueObjects.LocalizedValue
    {
      CzechValue = "Test",
      EnglishValue = "Test"
    };

    // Act
    var result1 = UseCases.LocalizedValueMapper.MapToDTO(domainEntity);
    var result2 = UseCases.LocalizedValueMapper.MapToDTO(domainEntity);

    // Assert
    result1.CzechValue.ShouldBe(result2.CzechValue);
    result1.EnglishValue.ShouldBe(result2.EnglishValue);
  }

  [Fact]
  public void MapToEntity_ShouldCreateEqualObjects_ForSameInput()
  {
    // Arrange
    var dto = new Contracts.DTO.LocalizedValue
    {
      CzechValue = "Test",
      EnglishValue = "Test"
    };

    // Act
    var result1 = UseCases.LocalizedValueMapper.MapToEntity(dto);
    var result2 = UseCases.LocalizedValueMapper.MapToEntity(dto);

    // Assert
    result1.Equals(result2).ShouldBeTrue();
  }

  #endregion
}
