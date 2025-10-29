using System;
using System.Collections.Generic;
using System.Linq;
using ASOL.DataService.Contracts;
using AVAIntegrationModeler.AVAPlace.Mapping;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using Xunit;

namespace AVAIntegrationModeler.AVAPlace.UnitTests.Mapping;

/// <summary>
/// Unit testy pro <see cref="DataModelMapper"/>.
/// </summary>
public class DataModelMapperTests
{
  [Fact]
  public void MapToDTO_WithValidDataModelDefinition_ReturnsMappedDTO()
  {
    // Arrange
    var dataModelDefinition = new DataModelDefinition
    {
      Id = Guid.NewGuid(),
      Code = "TestCode",
      Name = "Test Model",
      Description = "Test Description",
      Notes = "Test Notes",
      IsAggregateRoot = true,
      Fields = new List<DataModelFieldDefinition>
      {
        new DataModelFieldDefinition
        {
          Name = "TestField",
          Label = "Test Label",
          Description = "Field Description",
          IsPublishedForLookup = true,
          IsCollection = false,
          IsLocalized = false,
          IsNullable = true,
          FieldType = ASOL.DataService.Domain.Model.DataModelFieldType.Text,
          ReferencedEntityTypeIds = new List<Guid>()
        }
      }
    };

    // Act
    var result = DataModelMapper.MapToDTO(dataModelDefinition);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(dataModelDefinition.Id, result.Id);
    Assert.Equal(dataModelDefinition.Code, result.Code);
    Assert.Equal(dataModelDefinition.Name, result.Name);
    Assert.Equal(dataModelDefinition.Description, result.Description);
    Assert.Equal(dataModelDefinition.Notes, result.Notes);
    Assert.Equal(dataModelDefinition.IsAggregateRoot, result.IsAggregateRoot);
    Assert.Single(result.Fields);
    Assert.Equal("TestField", result.Fields[0].Name);
  }

  [Fact]
  public void MapToDTO_WithNullDataModelDefinition_ThrowsArgumentNullException()
  {
    // Act & Assert
    Assert.Throws<ArgumentNullException>(() => DataModelMapper.MapToDTO(null!));
  }

  [Fact]
  public void MapToDTO_WithNullFields_ReturnsEmptyFieldsList()
  {
    // Arrange
    var dataModelDefinition = new DataModelDefinition
    {
      Id = Guid.NewGuid(),
      Code = "TestCode",
      Name = "Test Model",
      Fields = null
    };

    // Act
    var result = DataModelMapper.MapToDTO(dataModelDefinition);

    // Assert
    Assert.NotNull(result.Fields);
    Assert.Empty(result.Fields);
  }

  [Fact]
  public void MapToSummaryDTO_WithValidDataModelDefinition_ReturnsMappedSummaryDTO()
  {
    // Arrange
    var dataModelDefinition = new DataModelDefinition
    {
      Id = Guid.NewGuid(),
      Code = "TestCode",
      Name = "Test Model",
      Description = "This should not be in summary"
    };

    // Act
    var result = DataModelMapper.MapToSummaryDTO(dataModelDefinition);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(dataModelDefinition.Id, result.Id);
    Assert.Equal(dataModelDefinition.Code, result.Code);
    Assert.Equal(dataModelDefinition.Name, result.Name);
  }

  [Fact]
  public void MapToSummaryDTO_WithNullDataModelDefinition_ThrowsArgumentNullException()
  {
    // Act & Assert
    Assert.Throws<ArgumentNullException>(() => DataModelMapper.MapToSummaryDTO(null!));
  }

  [Theory]
  [InlineData(ASOL.DataService.Domain.Model.DataModelFieldType.Text, DataModelFieldType.Text)]
  [InlineData(ASOL.DataService.Domain.Model.DataModelFieldType.MultilineText, DataModelFieldType.MultilineText)]
  [InlineData(ASOL.DataService.Domain.Model.DataModelFieldType.TwoOptions, DataModelFieldType.TwoOptions)]
  [InlineData(ASOL.DataService.Domain.Model.DataModelFieldType.WholeNumber, DataModelFieldType.WholeNumber)]
  [InlineData(ASOL.DataService.Domain.Model.DataModelFieldType.DecimalNumber, DataModelFieldType.DecimalNumber)]
  [InlineData(ASOL.DataService.Domain.Model.DataModelFieldType.UniqueIdentifier, DataModelFieldType.UniqueIdentifier)]
  [InlineData(ASOL.DataService.Domain.Model.DataModelFieldType.UtcDateTime, DataModelFieldType.UtcDateTime)]
  [InlineData(ASOL.DataService.Domain.Model.DataModelFieldType.LookupEntity, DataModelFieldType.LookupEntity)]
  [InlineData(ASOL.DataService.Domain.Model.DataModelFieldType.NestedEntity, DataModelFieldType.NestedEntity)]
  [InlineData(ASOL.DataService.Domain.Model.DataModelFieldType.Date, DataModelFieldType.Date)]
  [InlineData(ASOL.DataService.Domain.Model.DataModelFieldType.FileReference, DataModelFieldType.FileReference)]
  [InlineData(ASOL.DataService.Domain.Model.DataModelFieldType.CurrencyNumber, DataModelFieldType.CurrencyNumber)]
  [InlineData(ASOL.DataService.Domain.Model.DataModelFieldType.SingleSelectOptionSet, DataModelFieldType.SingleSelectOptionSet)]
  [InlineData(ASOL.DataService.Domain.Model.DataModelFieldType.MultiSelectOptionSet, DataModelFieldType.MultiSelectOptionSet)]
  public void MapToDTO_MapsFieldTypeCorrectly(ASOL.DataService.Domain.Model.DataModelFieldType sourceType, DataModelFieldType expectedType)
  {
    // Arrange
    var dataModelDefinition = new DataModelDefinition
    {
      Id = Guid.NewGuid(),
      Code = "TestCode",
      Name = "Test Model",
      Fields = new List<DataModelFieldDefinition>
      {
        new DataModelFieldDefinition
        {
          Name = "TestField",
          FieldType = sourceType
        }
      }
    };

    // Act
    var result = DataModelMapper.MapToDTO(dataModelDefinition);

    // Assert
    Assert.Equal(expectedType, result.Fields[0].FieldType);
  }

  [Fact]
  public void MapToDTO_WithReferencedEntityTypeIds_MapsList()
  {
    // Arrange
    var entityId1 = Guid.NewGuid();
    var entityId2 = Guid.NewGuid();
    var dataModelDefinition = new DataModelDefinition
    {
      Id = Guid.NewGuid(),
      Code = "TestCode",
      Name = "Test Model",
      Fields = new List<DataModelFieldDefinition>
      {
        new DataModelFieldDefinition
        {
          Name = "LookupField",
          FieldType = ASOL.DataService.Domain.Model.DataModelFieldType.LookupEntity,
          ReferencedEntityTypeIds = new List<Guid> { entityId1, entityId2 }
        }
      }
    };

    // Act
    var result = DataModelMapper.MapToDTO(dataModelDefinition);

    // Assert
    Assert.Equal(2, result.Fields[0].ReferencedEntityTypeIds.Count);
    Assert.Contains(entityId1, result.Fields[0].ReferencedEntityTypeIds);
    Assert.Contains(entityId2, result.Fields[0].ReferencedEntityTypeIds);
  }

  [Fact]
  public void MapToDTO_WithNullReferencedEntityTypeIds_ReturnsEmptyList()
  {
    // Arrange
    var dataModelDefinition = new DataModelDefinition
    {
      Id = Guid.NewGuid(),
      Code = "TestCode",
      Name = "Test Model",
      Fields = new List<DataModelFieldDefinition>
      {
        new DataModelFieldDefinition
        {
          Name = "TestField",
          FieldType = ASOL.DataService.Domain.Model.DataModelFieldType.Text,
          ReferencedEntityTypeIds = null
        }
      }
    };

    // Act
    var result = DataModelMapper.MapToDTO(dataModelDefinition);

    // Assert
    Assert.NotNull(result.Fields[0].ReferencedEntityTypeIds);
    Assert.Empty(result.Fields[0].ReferencedEntityTypeIds);
  }
}