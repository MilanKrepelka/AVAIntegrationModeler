using System;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.AreaAggregate;
using AVAIntegrationModeler.UseCases.Areas.Mapping;
using Shouldly;
using Xunit;

namespace AVAIntegrationModeler.UseCases.Test.Areas.Mapping;

/// <summary>
/// Unit testy pro AreaMapper.
/// </summary>
public class AreaMapperTests
{
  [Fact]
  public void MapToAreaDTO_ShouldReturnMappedDTO_WhenAreaIsValid()
  {
    // Arrange
    var areaId = Guid.NewGuid();
    var area = new Area(areaId, "SALES")
      .SetName("Sales Department");

    // Act
    var result = AreaMapper.MapToAreaDTO(area);

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldBe(areaId);
    result.Code.ShouldBe("SALES");
    result.Name.ShouldBe("Sales Department");
  }

  [Fact]
  public void MapToAreaDTO_ShouldReturnNull_WhenAreaIsNull()
  {
    // Arrange
    Area? area = null;

    // Act
    var result = AreaMapper.MapToAreaDTO(area);

    // Assert
    result.ShouldBeNull();
  }

  [Fact]
  public void MapToAreaDTO_ShouldHandleAreaWithoutName()
  {
    // Arrange
    var areaId = Guid.NewGuid();
    var area = new Area(areaId, "TECH");

    // Act
    var result = AreaMapper.MapToAreaDTO(area);

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldBe(areaId);
    result.Code.ShouldBe("TECH");
    result.Name.ShouldBe(string.Empty);
  }

  [Fact]
  public void MapToEntity_ShouldReturnMappedArea_WhenDTOIsValid()
  {
    // Arrange
    var dto = new AreaDTO
    {
      Id = Guid.NewGuid(),
      Code = "HR",
      Name = "Human Resources"
    };

    // Act
    var result = AreaMapper.MapToEntity(dto);

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldBe(dto.Id);
    result.Code.ShouldBe(dto.Code);
    result.Name.ShouldBe(dto.Name);
  }

  [Fact]
  public void MapToEntity_ShouldReturnNull_WhenDTOIsNull()
  {
    // Arrange
    AreaDTO? dto = null;

    // Act
    var result = AreaMapper.MapToEntity(dto);

    // Assert
    result.ShouldBeNull();
  }

  [Fact]
  public void MapToEntity_ShouldHandleDTOWithEmptyName()
  {
    // Arrange
    var dto = new AreaDTO
    {
      Id = Guid.NewGuid(),
      Code = "IT",
      Name = string.Empty
    };

    // Act
    var result = AreaMapper.MapToEntity(dto);

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldBe(dto.Id);
    result.Code.ShouldBe(dto.Code);
    result.Name.ShouldBe(string.Empty); // SetName by neměl být volán pro prázdný string
  }

  [Fact]
  public void MapToEntity_ShouldHandleDTOWithNullName()
  {
    // Arrange
    var dto = new AreaDTO
    {
      Id = Guid.NewGuid(),
      Code = "FIN",
      Name = null!
    };

    // Act
    var result = AreaMapper.MapToEntity(dto);

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldBe(dto.Id);
    result.Code.ShouldBe(dto.Code);
    result.Name.ShouldBe(string.Empty);
  }

  [Fact]
  public void MapToAreaDTO_AndBack_ShouldPreserveValues()
  {
    // Arrange
    var originalArea = new Area(Guid.NewGuid(), "OPS")
      .SetName("Operations");

    // Act
    var dto = AreaMapper.MapToAreaDTO(originalArea);
    var mappedBackArea = AreaMapper.MapToEntity(dto);

    // Assert
    mappedBackArea.ShouldNotBeNull();
    mappedBackArea.Id.ShouldBe(originalArea.Id);
    mappedBackArea.Code.ShouldBe(originalArea.Code);
    mappedBackArea.Name.ShouldBe(originalArea.Name);
  }
}
