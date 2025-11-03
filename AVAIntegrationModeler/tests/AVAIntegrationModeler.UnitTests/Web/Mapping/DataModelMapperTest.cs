using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Web.Mapping;
using AVAIntegrationModeler.Web.ViewModels.List;
using Shouldly;
using Xunit;

namespace AVAIntegrationModeler.UnitTests.Web.Mapping;

public class DataModelMapperTest
{
  [Fact]
  public void MapToViewModel_ShouldMapAllProperties_WhenDtoIsValid()
  {
    // Arrange
    var dataModelDto = new DataModelDTO
    {
      Id = Guid.NewGuid(),
      Code = "CUSTOMER",
      Name = "Customer",
      Description = "Customer data model",
      Notes = "Main customer aggregate",
      IsAggregateRoot = true,
      AreaId = Guid.NewGuid(),
      Fields = new List<DataModelFieldDTO>
      {
        new DataModelFieldDTO
        {
          Id = Guid.NewGuid(),
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
    result.Code.ShouldBe(dataModelDto.Code);
    result.Name.ShouldNotBeNull();
    result.Name.CzechValue.ShouldBe(dataModelDto.Name);
    result.Name.EnglishValue.ShouldBe(dataModelDto.Name);
    result.Description.ShouldNotBeNull();
    result.Description.CzechValue.ShouldBe(dataModelDto.Description);
    result.Description.EnglishValue.ShouldBe(dataModelDto.Description);
    result.Notes.ShouldBe(dataModelDto.Notes);
    result.IsAggregateRoot.ShouldBe(dataModelDto.IsAggregateRoot);
    result.AreaId.ShouldBe(dataModelDto.AreaId);
    result.Fields.ShouldNotBeNull();
    result.Fields.Count.ShouldBe(1);
  }

  [Fact]
  public void MapToViewModel_ShouldThrowException_WhenDtoIsNull()
  {
    // Arrange
    DataModelDTO? dto = null;
    var dataModels = new List<DataModelDTO>();

    // Act & Assert
    Should.Throw<ArgumentNullException>(() => DataModelMapper.MapToViewModel(dto!, dataModels));
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
  }

  [Fact]
  public void MapToViewModel_ShouldMapEmptyStringsToEmptyLocalizedValue_WhenNameIsEmpty()
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
    result.Name.ShouldBe(AVAIntegrationModeler.Contracts.DTO.LocalizedValue.Empty);
    result.Description.ShouldBe(AVAIntegrationModeler.Contracts.DTO.LocalizedValue.Empty);
  }

  [Fact]
  public void MapFieldToViewModel_ShouldMapAllFieldProperties_WhenDtoIsValid()
  {
    // Arrange
    var fieldDto = new DataModelFieldDTO
    {
      Id = Guid.NewGuid(),
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
    result.Id.ShouldBe(fieldDto.Id);
    result.Name.ShouldBe(fieldDto.Name);
    result.Label.ShouldBe(fieldDto.Label);
    result.Description.ShouldBe(fieldDto.Description);
    result.IsPublishedForLookup.ShouldBe(fieldDto.IsPublishedForLookup);
    result.IsCollection.ShouldBe(fieldDto.IsCollection);
    result.IsLocalized.ShouldBe(fieldDto.IsLocalized);
    result.IsNullable.ShouldBe(fieldDto.IsNullable);
    result.FieldType.ShouldBe(fieldDto.FieldType);
    result.ReferencedEntityTypeIds.ShouldBe(fieldDto.ReferencedEntityTypeIds);
    result.ReferencedModels.ShouldNotBeNull();
    result.ReferencedModels.Count.ShouldBe(0);
  }

  [Fact]
  public void MapFieldToViewModel_ShouldThrowException_WhenFieldDtoIsNull()
  {
    // Arrange
    DataModelFieldDTO? fieldDto = null;
    var dataModels = new List<DataModelDTO>();

    // Act & Assert
    Should.Throw<ArgumentNullException>(() => DataModelMapper.MapFieldToViewModel(fieldDto!, dataModels));
  }

  [Fact]
  public void MapFieldToViewModel_ShouldMapReferencedModels_WhenReferencedEntityTypeIdsExist()
  {
    // Arrange
    var referencedModelId1 = Guid.NewGuid();
    var referencedModelId2 = Guid.NewGuid();

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
        Id = Guid.NewGuid(),
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
    result.Fields[1].Name.ShouldBe("OrderDate");
    result.Fields[2].Name.ShouldBe("TotalAmount");
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

    var dataModels = new List<DataModelDTO>();

    // Act
    var result = DataModelMapper.MapFieldToViewModel(fieldDto, dataModels);

    // Assert
    result.ReferencedEntityTypeIds.ShouldNotBeNull();
    result.ReferencedEntityTypeIds.Count.ShouldBe(0);
    result.ReferencedModels.ShouldNotBeNull();
    result.ReferencedModels.Count.ShouldBe(0);
  }
}
