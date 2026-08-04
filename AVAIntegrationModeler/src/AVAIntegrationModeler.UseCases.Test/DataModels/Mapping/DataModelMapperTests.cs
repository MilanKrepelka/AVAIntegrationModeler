using System;
using System.Collections.Generic;
using System.Linq;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.DataModelAggregate;
using AVAIntegrationModeler.UseCases.DataModels.Mapping;
using Shouldly;
using Xunit;

namespace AVAIntegrationModeler.UseCases.Test.DataModels.Mapping;

/// <summary>
/// Unit testy pro DataModelMapper.
/// </summary>
public class DataModelMapperTests
{
  #region MapToDataModelSummaryDTO Tests

  [Fact]
  public void MapToDataModelSummaryDTO_ShouldThrow_WhenDataModelIsNull()
  {
    // Arrange
    DataModel? dataModel = null;

    // Act & Assert
    Should.Throw<ArgumentNullException>(() => DataModelMapper.MapToDataModelSummaryDTO(dataModel!));
  }

  [Fact]
  public void MapToDataModelSummaryDTO_ShouldReturnMappedDTO_WhenDataModelIsValid()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var dataModel = new DataModel(modelId, "MODEL-001");
    dataModel.SetName("Test Model");

    // Act
    var result = DataModelMapper.MapToDataModelSummaryDTO(dataModel);

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldBe(modelId);
    result.Code.ShouldBe("MODEL-001");
    result.Name.ShouldBe("Test Model");
  }

  [Fact]
  public void MapToDataModelSummaryDTO_ShouldMapOnlyIdCodeAndName()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var dataModel = new DataModel(modelId, "MODEL-FULL")
      .SetName("Full Model")
      .SetDescription("Detailed description")
      .SetNotes("Some notes")
      .MarkAsAggregateRoot();

    // Act
    var result = DataModelMapper.MapToDataModelSummaryDTO(dataModel);

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldBe(modelId);
    result.Code.ShouldBe("MODEL-FULL");
    result.Name.ShouldBe("Full Model");
    // DataModelSummaryDTO should NOT contain Description, Notes, IsAggregateRoot
  }

  [Fact]
  public void MapToDataModelSummaryDTO_ShouldHandleEmptyName()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var dataModel = new DataModel(modelId, "MODEL-NO-NAME");

    // Act
    var result = DataModelMapper.MapToDataModelSummaryDTO(dataModel);

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldBe(modelId);
    result.Code.ShouldBe("MODEL-NO-NAME");
    result.Name.ShouldBe(string.Empty);
  }

  [Fact]
  public void MapToDataModelSummaryDTO_ShouldHandleSpecialCharactersInCode()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var dataModel = new DataModel(modelId, "MODEL-ČŘŽ-123");
    dataModel.SetName("Český název");

    // Act
    var result = DataModelMapper.MapToDataModelSummaryDTO(dataModel);

    // Assert
    result.ShouldNotBeNull();
    result.Code.ShouldBe("MODEL-ČŘŽ-123");
    result.Name.ShouldBe("Český název");
  }

  #endregion

  #region MapToDataModelDTO Tests

  [Fact]
  public void MapToDataModelDTO_ShouldThrow_WhenDataModelIsNull()
  {
    // Arrange
    DataModel? dataModel = null;

    // Act & Assert
    Should.Throw<ArgumentNullException>(() => DataModelMapper.MapToDataModelDTO(dataModel!));
  }

  [Fact]
  public void MapToDataModelDTO_ShouldMapBasicProperties_WhenDataModelIsValid()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var areaId = Guid.NewGuid();
    var dataModel = new DataModel(modelId, "MODEL-001")
      .SetName("Test Model")
      .SetDescription("Test Description")
      .SetNotes("Test Notes")
      .MarkAsAggregateRoot()
      .SetArea(areaId);

    // Act
    var result = DataModelMapper.MapToDataModelDTO(dataModel);

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldBe(modelId);
    result.Code.ShouldBe("MODEL-001");
    result.Name.ShouldBe("Test Model");
    result.Description.ShouldBe("Test Description");
    result.Notes.ShouldBe("Test Notes");
    result.IsAggregateRoot.ShouldBeTrue();
    result.AreaId.ShouldBe(areaId);
  }

  [Fact]
  public void MapToDataModelDTO_ShouldMapEmptyFieldsCollection_WhenNoFields()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var dataModel = new DataModel(modelId, "MODEL-NO-FIELDS");

    // Act
    var result = DataModelMapper.MapToDataModelDTO(dataModel);

    // Assert
    result.ShouldNotBeNull();
    result.Fields.ShouldNotBeNull();
    result.Fields.ShouldBeEmpty();
  }

  [Fact]
  public void MapToDataModelDTO_ShouldMapFields_WhenFieldsExist()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var fieldId = Guid.NewGuid();
    var dataModel = new DataModel(modelId, "MODEL-WITH-FIELDS");
    
    var field = new DataModelField(fieldId, "TestField", DataModelFieldType.Text)
      .SetLabel("Test Label")
      .SetDescription("Test Field Description");
    
    dataModel.AddField(field);

    // Act
    var result = DataModelMapper.MapToDataModelDTO(dataModel);

    // Assert
    result.ShouldNotBeNull();
    result.Fields.ShouldNotBeNull();
    result.Fields.Count.ShouldBe(1);
    result.Fields.First().Id.ShouldBe(fieldId);
    result.Fields.First().Name.ShouldBe("TestField");
  }

  [Fact]
  public void MapToDataModelDTO_ShouldMapMultipleFields()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var dataModel = new DataModel(modelId, "MODEL-MULTI-FIELDS");
    
    var field1 = new DataModelField(Guid.NewGuid(), "Field1", DataModelFieldType.Text);
    var field2 = new DataModelField(Guid.NewGuid(), "Field2", DataModelFieldType.WholeNumber);
    var field3 = new DataModelField(Guid.NewGuid(), "Field3", DataModelFieldType.TwoOptions);
    
    dataModel.AddField(field1);
    dataModel.AddField(field2);
    dataModel.AddField(field3);

    // Act
    var result = DataModelMapper.MapToDataModelDTO(dataModel);

    // Assert
    result.Fields.Count.ShouldBe(3);
    result.Fields.ShouldContain(f => f.Name == "Field1");
    result.Fields.ShouldContain(f => f.Name == "Field2");
    result.Fields.ShouldContain(f => f.Name == "Field3");
  }

  [Fact]
  public void MapToDataModelDTO_ShouldHandleNestedEntity()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var dataModel = new DataModel(modelId, "NESTED-MODEL")
      .SetName("Nested Entity")
      .MarkAsNestedEntity();

    // Act
    var result = DataModelMapper.MapToDataModelDTO(dataModel);

    // Assert
    result.ShouldNotBeNull();
    result.IsAggregateRoot.ShouldBeFalse();
  }

  [Fact]
  public void MapToDataModelDTO_ShouldHandleNullAreaId()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var dataModel = new DataModel(modelId, "MODEL-NO-AREA");

    // Act
    var result = DataModelMapper.MapToDataModelDTO(dataModel);

    // Assert
    result.AreaId.ShouldBeNull();
  }

  #endregion

  #region MapToEntity Tests

  [Fact]
  public void MapToEntity_ShouldThrow_WhenDtoIsNull()
  {
    // Arrange
    DataModelDTO? dto = null;

    // Act & Assert
    Should.Throw<ArgumentNullException>(() => DataModelMapper.MapToEntity(dto!));
  }

  [Fact]
  public void MapToEntity_ShouldMapBasicProperties_WhenDtoIsValid()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var areaId = Guid.NewGuid();
    var dto = new DataModelDTO
    {
      Id = modelId,
      Code = "MODEL-001",
      Name = "Test Model",
      Description = "Test Description",
      Notes = "Test Notes",
      IsAggregateRoot = true,
      AreaId = areaId,
      Fields = new List<DataModelFieldDTO>()
    };

    // Act
    var result = DataModelMapper.MapToEntity(dto);

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldBe(modelId);
    result.Code.ShouldBe("MODEL-001");
    result.Name.ShouldBe("Test Model");
    result.Description.ShouldBe("Test Description");
    result.Notes.ShouldBe("Test Notes");
    result.IsAggregateRoot.ShouldBeTrue();
    result.AreaId.ShouldBe(areaId);
  }

  [Fact]
  public void MapToEntity_ShouldSkipEmptyName()
  {
    // Arrange
    var dto = new DataModelDTO
    {
      Id = Guid.NewGuid(),
      Code = "MODEL-EMPTY-NAME",
      Name = string.Empty,
      Description = "Description",
      Fields = new List<DataModelFieldDTO>()
    };

    // Act
    var result = DataModelMapper.MapToEntity(dto);

    // Assert
    result.Name.ShouldBe(string.Empty);
  }

  [Fact]
  public void MapToEntity_ShouldSkipEmptyDescription()
  {
    // Arrange
    var dto = new DataModelDTO
    {
      Id = Guid.NewGuid(),
      Code = "MODEL-EMPTY-DESC",
      Name = "Test",
      Description = string.Empty,
      Fields = new List<DataModelFieldDTO>()
    };

    // Act
    var result = DataModelMapper.MapToEntity(dto);

    // Assert
    result.Description.ShouldBe(string.Empty);
  }

  [Fact]
  public void MapToEntity_ShouldSkipEmptyNotes()
  {
    // Arrange
    var dto = new DataModelDTO
    {
      Id = Guid.NewGuid(),
      Code = "MODEL-EMPTY-NOTES",
      Name = "Test",
      Notes = string.Empty,
      Fields = new List<DataModelFieldDTO>()
    };

    // Act
    var result = DataModelMapper.MapToEntity(dto);

    // Assert
    result.Notes.ShouldBe(string.Empty);
  }

  [Fact]
  public void MapToEntity_ShouldMarkAsNestedEntity_WhenIsAggregateRootIsFalse()
  {
    // Arrange
    var dto = new DataModelDTO
    {
      Id = Guid.NewGuid(),
      Code = "NESTED-MODEL",
      Name = "Nested",
      IsAggregateRoot = false,
      Fields = new List<DataModelFieldDTO>()
    };

    // Act
    var result = DataModelMapper.MapToEntity(dto);

    // Assert
    result.IsAggregateRoot.ShouldBeFalse();
  }

  [Fact]
  public void MapToEntity_ShouldHandleNullAreaId()
  {
    // Arrange
    var dto = new DataModelDTO
    {
      Id = Guid.NewGuid(),
      Code = "MODEL-NO-AREA",
      Name = "Test",
      AreaId = null,
      Fields = new List<DataModelFieldDTO>()
    };

    // Act
    var result = DataModelMapper.MapToEntity(dto);

    // Assert
    result.AreaId.ShouldBeNull();
  }

  [Fact]
  public void MapToEntity_ShouldMapFields_WhenFieldsExist()
  {
    // Arrange
    var fieldId = Guid.NewGuid();
    var dto = new DataModelDTO
    {
      Id = Guid.NewGuid(),
      Code = "MODEL-WITH-FIELDS",
      Name = "Test",
      Fields = new List<DataModelFieldDTO>
      {
        new DataModelFieldDTO
        {
          Id = fieldId,
          Name = "TestField",
          Label = "Test Label",
          Description = "Test Description",
          FieldType = DataModelFieldType.Text,
          IsPublishedForLookup = false,
          IsCollection = false,
          IsLocalized = false,
          IsNullable = true,
          ReferencedEntityTypeIds = new List<Guid>()
        }
      }
    };

    // Act
    var result = DataModelMapper.MapToEntity(dto);

    // Assert
    result.Fields.Count.ShouldBe(1);
    result.Fields.First().Id.ShouldBe(fieldId);
    result.Fields.First().Name.ShouldBe("TestField");
  }

  [Fact]
  public void MapToEntity_ShouldMapMultipleFields()
  {
    // Arrange
    var dto = new DataModelDTO
    {
      Id = Guid.NewGuid(),
      Code = "MODEL-MULTI-FIELDS",
      Name = "Test",
      Fields = new List<DataModelFieldDTO>
      {
        new DataModelFieldDTO
        {
          Id = Guid.NewGuid(),
          Name = "Field1",
          FieldType = DataModelFieldType.Text,
          ReferencedEntityTypeIds = new List<Guid>()
        },
        new DataModelFieldDTO
        {
          Id = Guid.NewGuid(),
          Name = "Field2",
          FieldType = DataModelFieldType.WholeNumber,
          ReferencedEntityTypeIds = new List<Guid>()
        },
        new DataModelFieldDTO
        {
          Id = Guid.NewGuid(),
          Name = "Field3",
          FieldType = DataModelFieldType.TwoOptions,
          ReferencedEntityTypeIds = new List<Guid>()
        }
      }
    };

    // Act
    var result = DataModelMapper.MapToEntity(dto);

    // Assert
    result.Fields.Count.ShouldBe(3);
    result.Fields.ShouldContain(f => f.Name == "Field1");
    result.Fields.ShouldContain(f => f.Name == "Field2");
    result.Fields.ShouldContain(f => f.Name == "Field3");
  }

  [Fact]
  public void MapToEntity_ShouldSkipNullFields()
  {
    // Arrange
    var dto = new DataModelDTO
    {
      Id = Guid.NewGuid(),
      Code = "MODEL-NULL-FIELD",
      Name = "Test",
      Fields = new List<DataModelFieldDTO>()
    };

    // Act
    var result = DataModelMapper.MapToEntity(dto);

    // Assert
    result.Fields.ShouldBeEmpty();
  }

  #endregion

  #region Round-Trip Mapping Tests

  [Fact]
  public void RoundTrip_ShouldPreserveBasicProperties()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var areaId = Guid.NewGuid();
    var original = new DataModel(modelId, "ROUND-TRIP")
      .SetName("Original Model")
      .SetDescription("Original Description")
      .SetNotes("Original Notes")
      .MarkAsAggregateRoot()
      .SetArea(areaId);

    // Act
    var dto = DataModelMapper.MapToDataModelDTO(original);
    var result = DataModelMapper.MapToEntity(dto);

    // Assert
    result.Id.ShouldBe(original.Id);
    result.Code.ShouldBe(original.Code);
    result.Name.ShouldBe(original.Name);
    result.Description.ShouldBe(original.Description);
    result.Notes.ShouldBe(original.Notes);
    result.IsAggregateRoot.ShouldBe(original.IsAggregateRoot);
    result.AreaId.ShouldBe(original.AreaId);
  }

  [Fact]
  public void RoundTrip_ShouldPreserveFields()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var fieldId = Guid.NewGuid();
    var original = new DataModel(modelId, "ROUND-TRIP-FIELDS");
    
    var field = new DataModelField(fieldId, "TestField", DataModelFieldType.Text)
      .SetLabel("Test Label")
      .SetDescription("Test Description")
      .MarkAsNullable();
    
    original.AddField(field);

    // Act
    var dto = DataModelMapper.MapToDataModelDTO(original);
    var result = DataModelMapper.MapToEntity(dto);

    // Assert
    result.Fields.Count.ShouldBe(1);
    result.Fields.First().Id.ShouldBe(fieldId);
    result.Fields.First().Name.ShouldBe("TestField");
    result.Fields.First().Label.ShouldBe("Test Label");
    result.Fields.First().Description.ShouldBe("Test Description");
    result.Fields.First().IsNullable.ShouldBeTrue();
  }

  #endregion

  #region Consistency Tests

  [Fact]
  public void MapToDataModelSummaryDTO_ShouldBeConsistent_ForSameInput()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var dataModel = new DataModel(modelId, "CONSISTENT")
      .SetName("Consistent Model");

    // Act
    var result1 = DataModelMapper.MapToDataModelSummaryDTO(dataModel);
    var result2 = DataModelMapper.MapToDataModelSummaryDTO(dataModel);

    // Assert
    result1.Id.ShouldBe(result2.Id);
    result1.Code.ShouldBe(result2.Code);
    result1.Name.ShouldBe(result2.Name);
  }

  [Fact]
  public void MapToDataModelDTO_ShouldBeConsistent_ForSameInput()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var dataModel = new DataModel(modelId, "CONSISTENT-FULL")
      .SetName("Consistent Model")
      .SetDescription("Description");

    // Act
    var result1 = DataModelMapper.MapToDataModelDTO(dataModel);
    var result2 = DataModelMapper.MapToDataModelDTO(dataModel);

    // Assert
    result1.Id.ShouldBe(result2.Id);
    result1.Code.ShouldBe(result2.Code);
    result1.Name.ShouldBe(result2.Name);
    result1.Description.ShouldBe(result2.Description);
  }

  #endregion

  #region Edge Cases

  [Fact]
  public void MapToDataModelDTO_ShouldHandleLongStrings()
  {
    // Arrange
    var longString = new string('A', 1000);
    var dataModel = new DataModel(Guid.NewGuid(), "LONG")
      .SetName(longString)
      .SetDescription(longString)
      .SetNotes(longString);

    // Act
    var result = DataModelMapper.MapToDataModelDTO(dataModel);

    // Assert
    result.Name.Length.ShouldBe(1000);
    result.Description.Length.ShouldBe(1000);
    result.Notes.Length.ShouldBe(1000);
  }

  [Fact]
  public void MapToEntity_ShouldHandleMinimalDTO()
  {
    // Arrange
    var dto = new DataModelDTO
    {
      Id = Guid.NewGuid(),
      Code = "MINIMAL",
      Name = string.Empty,
      Description = string.Empty,
      Notes = string.Empty,
      IsAggregateRoot = false,
      AreaId = null,
      Fields = new List<DataModelFieldDTO>()
    };

    // Act
    var result = DataModelMapper.MapToEntity(dto);

    // Assert
    result.ShouldNotBeNull();
    result.Code.ShouldBe("MINIMAL");
    result.IsAggregateRoot.ShouldBeFalse();
    result.AreaId.ShouldBeNull();
    result.Fields.ShouldBeEmpty();
  }

  #endregion
}
