using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Web.Mapping;
using AVAIntegrationModeler.Web.ViewModels.List;
using Shouldly;
using Xunit;

namespace AVAIntegrationModeler.Web.Test.Mapping;

/// <summary>
/// Unit testy pro DataModelMapper.
/// </summary>
public class DataModelMapperTests
{
  #region MapToViewModel Tests

  [Fact]
  public void MapToViewModel_ShouldMapAllProperties_WhenDtoIsValid()
  {
    // Arrange
    var areaId = Guid.NewGuid();
    var fieldId = Guid.NewGuid();
    
    var dataModelDto = new DataModelDTO
    {
      Id = Guid.NewGuid(),
      Code = "CUSTOMER",
      Name = "Customer",
      Description = "Customer data model",
      Notes = "Main customer aggregate",
      IsAggregateRoot = true,
      AreaId = areaId,
      Fields = new List<DataModelFieldDTO>
      {
        new DataModelFieldDTO
        {
          Id = fieldId,
          Name = "CustomerId",
          Label = "Customer ID",
          Description = "Unique customer identifier",
          IsPublishedForLookup = true,
          IsCollection = false,
          IsLocalized = false,
          IsNullable = false,
          FieldType = DataModelFieldType.UniqueIdentifier,
          ReferencedEntityTypeIds = new List<Guid>()
        }
      }
    };

    var dataModels = new List<DataModelDTO> { dataModelDto };

    // Act
    var result = DataModelMapper.MapToViewModel(dataModelDto, dataModels);

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldBe(dataModelDto.Id);
    result.Code.ShouldBe("CUSTOMER");
    result.Name.ShouldNotBeNull();
    result.Name.CzechValue.ShouldBe("Customer");
    result.Name.EnglishValue.ShouldBe("Customer");
    result.Description.ShouldNotBeNull();
    result.Description.CzechValue.ShouldBe("Customer data model");
    result.Description.EnglishValue.ShouldBe("Customer data model");
    result.Notes.ShouldBe("Main customer aggregate");
    result.IsAggregateRoot.ShouldBeTrue();
    result.AreaId.ShouldBe(areaId);
    result.Fields.ShouldNotBeNull();
    result.Fields.Count.ShouldBe(1);
    result.Fields[0].Id.ShouldBe(fieldId);
  }

  [Fact]
  public void MapToViewModel_ShouldThrowArgumentNullException_WhenDtoIsNull()
  {
    // Arrange
    DataModelDTO? dto = null;
    var dataModels = new List<DataModelDTO>();

    // Act & Assert
    Should.Throw<ArgumentNullException>(() => 
      DataModelMapper.MapToViewModel(dto!, dataModels));
  }

  [Fact]
  public void MapToViewModel_ShouldHandleEmptyFields_WhenFieldsListIsEmpty()
  {
    // Arrange
    var dataModelDto = new DataModelDTO
    {
      Id = Guid.NewGuid(),
      Code = "EMPTY",
      Name = "Empty Model",
      Description = "Model without fields",
      Notes = "",
      IsAggregateRoot = false,
      AreaId = null,
      Fields = new List<DataModelFieldDTO>()
    };

    var dataModels = new List<DataModelDTO> { dataModelDto };

    // Act
    var result = DataModelMapper.MapToViewModel(dataModelDto, dataModels);

    // Assert
    result.ShouldNotBeNull();
    result.Fields.ShouldNotBeNull();
    result.Fields.Count.ShouldBe(0);
    result.AreaId.ShouldBeNull();
  }

  [Fact]
  public void MapToViewModel_ShouldMapToEmptyLocalizedValue_WhenNameIsEmpty()
  {
    // Arrange
    var dataModelDto = new DataModelDTO
    {
      Id = Guid.NewGuid(),
      Code = "TEST",
      Name = "",
      Description = "",
      Notes = "",
      IsAggregateRoot = false,
      AreaId = null,
      Fields = new List<DataModelFieldDTO>()
    };

    var dataModels = new List<DataModelDTO> { dataModelDto };

    // Act
    var result = DataModelMapper.MapToViewModel(dataModelDto, dataModels);

    // Assert
    result.Name.ShouldBe(LocalizedValue.Empty);
    result.Description.ShouldBe(LocalizedValue.Empty);
  }

  [Fact]
  public void MapToViewModel_ShouldMapToEmptyLocalizedValue_WhenNameIsNull()
  {
    // Arrange
    var dataModelDto = new DataModelDTO
    {
      Id = Guid.NewGuid(),
      Code = "TEST",
      Name = null!,
      Description = null!,
      Notes = "",
      IsAggregateRoot = false,
      AreaId = null,
      Fields = new List<DataModelFieldDTO>()
    };

    var dataModels = new List<DataModelDTO> { dataModelDto };

    // Act
    var result = DataModelMapper.MapToViewModel(dataModelDto, dataModels);

    // Assert
    result.Name.ShouldBe(LocalizedValue.Empty);
    result.Description.ShouldBe(LocalizedValue.Empty);
  }

  [Fact]
  public void MapToViewModel_ShouldHandleMultipleFields_WhenFieldsListContainsMultipleItems()
  {
    // Arrange
    var dataModelDto = new DataModelDTO
    {
      Id = Guid.NewGuid(),
      Code = "ORDER",
      Name = "Order",
      Description = "Order data model",
      Notes = "Contains order information",
      IsAggregateRoot = true,
      AreaId = Guid.NewGuid(),
      Fields = new List<DataModelFieldDTO>
      {
        new DataModelFieldDTO
        {
          Id = Guid.NewGuid(),
          Name = "OrderId",
          Label = "Order ID",
          Description = "Unique order identifier",
          IsPublishedForLookup = true,
          IsCollection = false,
          IsLocalized = false,
          IsNullable = false,
          FieldType = DataModelFieldType.UniqueIdentifier,
          ReferencedEntityTypeIds = new List<Guid>()
        },
        new DataModelFieldDTO
        {
          Id = Guid.NewGuid(),
          Name = "OrderDate",
          Label = "Order Date",
          Description = "Date when order was created",
          IsPublishedForLookup = false,
          IsCollection = false,
          IsLocalized = false,
          IsNullable = false,
          FieldType = DataModelFieldType.UtcDateTime,
          ReferencedEntityTypeIds = new List<Guid>()
        },
        new DataModelFieldDTO
        {
          Id = Guid.NewGuid(),
          Name = "TotalAmount",
          Label = "Total Amount",
          Description = "Total order amount",
          IsPublishedForLookup = false,
          IsCollection = false,
          IsLocalized = false,
          IsNullable = false,
          FieldType = DataModelFieldType.CurrencyNumber,
          ReferencedEntityTypeIds = new List<Guid>()
        }
      }
    };

    var dataModels = new List<DataModelDTO> { dataModelDto };

    // Act
    var result = DataModelMapper.MapToViewModel(dataModelDto, dataModels);

    // Assert
    result.Fields.ShouldNotBeNull();
    result.Fields.Count.ShouldBe(3);
    result.Fields[0].Name.ShouldBe("OrderId");
    result.Fields[0].FieldType.ShouldBe(DataModelFieldType.UniqueIdentifier);
    result.Fields[1].Name.ShouldBe("OrderDate");
    result.Fields[1].FieldType.ShouldBe(DataModelFieldType.UtcDateTime);
    result.Fields[2].Name.ShouldBe("TotalAmount");
    result.Fields[2].FieldType.ShouldBe(DataModelFieldType.CurrencyNumber);
  }

  #endregion

  #region MapFieldToViewModel Tests

  [Fact]
  public void MapFieldToViewModel_ShouldMapAllFieldProperties_WhenDtoIsValid()
  {
    // Arrange
    var fieldId = Guid.NewGuid();
    var fieldDto = new DataModelFieldDTO
    {
      Id = fieldId,
      Name = "CustomerId",
      Label = "Customer ID",
      Description = "Unique identifier",
      IsPublishedForLookup = true,
      IsCollection = false,
      IsLocalized = false,
      IsNullable = false,
      FieldType = DataModelFieldType.UniqueIdentifier,
      ReferencedEntityTypeIds = new List<Guid>()
    };

    var dataModels = new List<DataModelDTO>();

    // Act
    var result = DataModelMapper.MapFieldToViewModel(fieldDto, dataModels);

    // Assert
    result.ShouldNotBeNull();
    result.Id.ShouldBe(fieldId);
    result.Name.ShouldBe("CustomerId");
    result.Label.ShouldBe("Customer ID");
    result.Description.ShouldBe("Unique identifier");
    result.IsPublishedForLookup.ShouldBeTrue();
    result.IsCollection.ShouldBeFalse();
    result.IsLocalized.ShouldBeFalse();
    result.IsNullable.ShouldBeFalse();
    result.FieldType.ShouldBe(DataModelFieldType.UniqueIdentifier);
    result.ReferencedEntityTypeIds.ShouldNotBeNull();
    result.ReferencedEntityTypeIds.Count.ShouldBe(0);
    result.ReferencedModels.ShouldNotBeNull();
    result.ReferencedModels.Count.ShouldBe(0);
  }

  [Fact]
  public void MapFieldToViewModel_ShouldThrowArgumentNullException_WhenFieldDtoIsNull()
  {
    // Arrange
    DataModelFieldDTO? fieldDto = null;
    var dataModels = new List<DataModelDTO>();

    // Act & Assert
    Should.Throw<ArgumentNullException>(() => 
      DataModelMapper.MapFieldToViewModel(fieldDto!, dataModels));
  }

  [Fact]
  public void MapFieldToViewModel_ShouldMapReferencedModels_WhenReferencedEntityTypeIdsExist()
  {
    // Arrange
    var referencedModelId1 = Guid.NewGuid();
    var referencedModelId2 = Guid.NewGuid();
    var nonReferencedModelId = Guid.NewGuid();

    var fieldDto = new DataModelFieldDTO
    {
      Id = Guid.NewGuid(),
      Name = "RelatedEntities",
      Label = "Related Entities",
      Description = "References to other entities",
      IsPublishedForLookup = false,
      IsCollection = true,
      IsLocalized = false,
      IsNullable = true,
      FieldType = DataModelFieldType.LookupEntity,
      ReferencedEntityTypeIds = new List<Guid> { referencedModelId1, referencedModelId2 }
    };

    var dataModels = new List<DataModelDTO>
    {
      new DataModelDTO
      {
        Id = referencedModelId1,
        Code = "MODEL1",
        Name = "Model 1",
        Description = "First model",
        Notes = "",
        IsAggregateRoot = true,
        AreaId = null,
        Fields = new List<DataModelFieldDTO>()
      },
      new DataModelDTO
      {
        Id = referencedModelId2,
        Code = "MODEL2",
        Name = "Model 2",
        Description = "Second model",
        Notes = "",
        IsAggregateRoot = false,
        AreaId = null,
        Fields = new List<DataModelFieldDTO>()
      },
      new DataModelDTO
      {
        Id = nonReferencedModelId,
        Code = "MODEL3",
        Name = "Model 3",
        Description = "Third model - not referenced",
        Notes = "",
        IsAggregateRoot = false,
        AreaId = null,
        Fields = new List<DataModelFieldDTO>()
      }
    };

    // Act
    var result = DataModelMapper.MapFieldToViewModel(fieldDto, dataModels);

    // Assert
    result.ReferencedModels.ShouldNotBeNull();
    result.ReferencedModels.Count.ShouldBe(2);
    result.ReferencedModels.ShouldContain(dm => dm.Id == referencedModelId1);
    result.ReferencedModels.ShouldContain(dm => dm.Id == referencedModelId2);
    result.ReferencedModels.ShouldNotContain(dm => dm.Id == nonReferencedModelId);
    result.ReferencedModels[0].Code.ShouldBe("MODEL1");
    result.ReferencedModels[1].Code.ShouldBe("MODEL2");
  }

  [Fact]
  public void MapFieldToViewModel_ShouldHandleEmptyReferencedEntityTypeIds_WhenListIsEmpty()
  {
    // Arrange
    var fieldDto = new DataModelFieldDTO
    {
      Id = Guid.NewGuid(),
      Name = "SimpleField",
      Label = "Simple Field",
      Description = "A simple field without references",
      IsPublishedForLookup = false,
      IsCollection = false,
      IsLocalized = false,
      IsNullable = true,
      FieldType = DataModelFieldType.Text,
      ReferencedEntityTypeIds = new List<Guid>()
    };

    var dataModels = new List<DataModelDTO>
    {
      new DataModelDTO
      {
        Id = Guid.NewGuid(),
        Code = "MODEL1",
        Name = "Model 1",
        Description = "Some model",
        Notes = "",
        IsAggregateRoot = true,
        AreaId = null,
        Fields = new List<DataModelFieldDTO>()
      }
    };

    // Act
    var result = DataModelMapper.MapFieldToViewModel(fieldDto, dataModels);

    // Assert
    result.ReferencedEntityTypeIds.ShouldNotBeNull();
    result.ReferencedEntityTypeIds.Count.ShouldBe(0);
    result.ReferencedModels.ShouldNotBeNull();
    result.ReferencedModels.Count.ShouldBe(0);
  }

  [Fact]
  public void MapFieldToViewModel_ShouldMapAllFieldTypes_WhenDifferentTypesAreProvided()
  {
    // Arrange
    var dataModels = new List<DataModelDTO>();

    var testCases = new[]
    {
      DataModelFieldType.Text,
      DataModelFieldType.MultilineText,
      DataModelFieldType.TwoOptions,
      DataModelFieldType.WholeNumber,
      DataModelFieldType.DecimalNumber,
      DataModelFieldType.UniqueIdentifier,
      DataModelFieldType.UtcDateTime,
      DataModelFieldType.Date,
      DataModelFieldType.FileReference,
      DataModelFieldType.CurrencyNumber,
      DataModelFieldType.SingleSelectOptionSet,
      DataModelFieldType.MultiSelectOptionSet,
      DataModelFieldType.LookupEntity,
      DataModelFieldType.NestedEntity
    };

    foreach (var fieldType in testCases)
    {
      var fieldDto = new DataModelFieldDTO
      {
        Id = Guid.NewGuid(),
        Name = $"Field_{fieldType}",
        Label = $"Field {fieldType}",
        Description = $"Field of type {fieldType}",
        IsPublishedForLookup = false,
        IsCollection = false,
        IsLocalized = false,
        IsNullable = true,
        FieldType = fieldType,
        ReferencedEntityTypeIds = new List<Guid>()
      };

      // Act
      var result = DataModelMapper.MapFieldToViewModel(fieldDto, dataModels);

      // Assert
      result.FieldType.ShouldBe(fieldType);
      result.Name.ShouldBe($"Field_{fieldType}");
    }
  }

  [Fact]
  public void MapFieldToViewModel_ShouldHandleBooleanFlags_WhenAllFlagsAreSet()
  {
    // Arrange
    var fieldDto = new DataModelFieldDTO
    {
      Id = Guid.NewGuid(),
      Name = "FlagField",
      Label = "Flag Field",
      Description = "Field with all flags set",
      IsPublishedForLookup = true,
      IsCollection = true,
      IsLocalized = true,
      IsNullable = true,
      FieldType = DataModelFieldType.Text,
      ReferencedEntityTypeIds = new List<Guid>()
    };

    var dataModels = new List<DataModelDTO>();

    // Act
    var result = DataModelMapper.MapFieldToViewModel(fieldDto, dataModels);

    // Assert
    result.IsPublishedForLookup.ShouldBeTrue();
    result.IsCollection.ShouldBeTrue();
    result.IsLocalized.ShouldBeTrue();
    result.IsNullable.ShouldBeTrue();
  }

  [Fact]
  public void MapFieldToViewModel_ShouldHandleBooleanFlags_WhenAllFlagsAreNotSet()
  {
    // Arrange
    var fieldDto = new DataModelFieldDTO
    {
      Id = Guid.NewGuid(),
      Name = "FlagField",
      Label = "Flag Field",
      Description = "Field with all flags not set",
      IsPublishedForLookup = false,
      IsCollection = false,
      IsLocalized = false,
      IsNullable = false,
      FieldType = DataModelFieldType.Text,
      ReferencedEntityTypeIds = new List<Guid>()
    };

    var dataModels = new List<DataModelDTO>();

    // Act
    var result = DataModelMapper.MapFieldToViewModel(fieldDto, dataModels);

    // Assert
    result.IsPublishedForLookup.ShouldBeFalse();
    result.IsCollection.ShouldBeFalse();
    result.IsLocalized.ShouldBeFalse();
    result.IsNullable.ShouldBeFalse();
  }

  #endregion
}
