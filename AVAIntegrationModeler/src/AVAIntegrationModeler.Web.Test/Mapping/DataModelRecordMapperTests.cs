using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Web.SyncfusionApp.Mapping;
using Shouldly;
using Xunit;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Test.Mapping;

/// <summary>
/// Unit testy pro DataModelRecordMapper.
/// </summary>
public class DataModelRecordMapperTests
{
  private static DataModelDTO NewModel(Guid id, string name) => new DataModelDTO
  {
    Id = id,
    Code = name.ToUpperInvariant(),
    Name = name,
    Description = "",
    Notes = "",
    IsAggregateRoot = false,
    Fields = new List<DataModelFieldDTO>()
  };

  [Fact]
  public void MapToViewModel_ShouldMapModelName_WhenModelExistsInList()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var recordDto = new DataModelRecordDTO
    {
      Id = Guid.NewGuid(),
      ModelId = modelId,
      ExternalId = "EXT-1",
      Fields = new List<DataModelRecordFieldDTO>()
    };
    var dataModels = new List<DataModelDTO> { NewModel(modelId, "Customer") };

    // Act
    var result = DataModelRecordMapper.MapToViewModel(recordDto, dataModels);

    // Assert
    result.ModelId.ShouldBe(modelId);
    result.ModelName.ShouldBe("Customer");
  }

  [Fact]
  public void MapToViewModel_ShouldReturnEmptyModelName_WhenModelNotFoundInList()
  {
    // Arrange
    var recordDto = new DataModelRecordDTO
    {
      Id = Guid.NewGuid(),
      ModelId = Guid.NewGuid(),
      ExternalId = "EXT-1",
      Fields = new List<DataModelRecordFieldDTO>()
    };
    var dataModels = new List<DataModelDTO> { NewModel(Guid.NewGuid(), "Unrelated") };

    // Act
    var result = DataModelRecordMapper.MapToViewModel(recordDto, dataModels);

    // Assert
    result.ModelName.ShouldBe(string.Empty);
  }

  [Fact]
  public void MapToViewModel_ShouldMapFields_WhenRecordHasFields()
  {
    // Arrange
    var modelId = Guid.NewGuid();
    var recordDto = new DataModelRecordDTO
    {
      Id = Guid.NewGuid(),
      ModelId = modelId,
      ExternalId = "EXT-1",
      Fields = new List<DataModelRecordFieldDTO>
      {
        new DataModelRecordFieldDTO { Key = "Nazev", IsLocalized = false, StringValue = "Hodnota" },
        new DataModelRecordFieldDTO { Key = "Popis", IsLocalized = true, CzechValue = "Cesky", EnglishValue = "English" }
      }
    };
    var dataModels = new List<DataModelDTO> { NewModel(modelId, "Order") };

    // Act
    var result = DataModelRecordMapper.MapToViewModel(recordDto, dataModels);

    // Assert
    result.Fields.Count.ShouldBe(2);
    result.Fields[0].Key.ShouldBe("Nazev");
    result.Fields[0].StringValue.ShouldBe("Hodnota");
    result.Fields[1].IsLocalized.ShouldBeTrue();
    result.Fields[1].CzechValue.ShouldBe("Cesky");
    result.Fields[1].EnglishValue.ShouldBe("English");
  }

  [Fact]
  public void MapToViewModel_ShouldReturnEmptyModelName_WhenDataModelsListIsEmpty()
  {
    // Arrange
    var recordDto = new DataModelRecordDTO
    {
      Id = Guid.NewGuid(),
      ModelId = Guid.NewGuid(),
      ExternalId = "EXT-1",
      Fields = new List<DataModelRecordFieldDTO>()
    };

    // Act
    var result = DataModelRecordMapper.MapToViewModel(recordDto, new List<DataModelDTO>());

    // Assert
    result.ModelName.ShouldBe(string.Empty);
  }
}
