using System;
using System.Collections.Generic;
using System.Linq;
using ASOL.Core.Localization;
using AVAIntegrationModeler.AVAPlace.Mapping;
using AVAIntegrationModeler.AVAPlace.Models;
using AVAIntegrationModeler.Contracts.DTO;
using Xunit;

namespace AVAIntegrationModeler.AVAPlace.UnitTests.Mapping;

/// <summary>
/// Unit testy pro <see cref="DataModelRecordMapper"/>.
/// </summary>
public class DataModelRecordMapperTests
{
  private static readonly Guid SampleModelId = Guid.Parse("35d31a32-1189-4e22-8e33-0a7d1128983c");
  private static readonly Guid SampleRecordId = Guid.Parse("50e6009b-db9a-420f-8cd3-ab75177b7137");

  // -------------------------------------------------------------------
  // MapToDTO — null a guard klauzule
  // -------------------------------------------------------------------

  [Fact]
  public void MapToDTO_NullRecord_ThrowsArgumentNullException()
  {
    Assert.Throws<ArgumentNullException>(() => DataModelRecordMapper.MapToDTO(null!));
  }

  [Fact]
  public void MapToDTO_NullId_ThrowsArgumentNullException()
  {
    var record = new DataModelRecord { Id = null! };
    Assert.Throws<ArgumentNullException>(() => DataModelRecordMapper.MapToDTO(record));
  }

  // -------------------------------------------------------------------
  // MapToDTO — mapování identifikátorů a metadat
  // -------------------------------------------------------------------

  [Fact]
  public void MapToDTO_ValidRecord_MapsRecordIdToId()
  {
    var record = BuildFullRecord();
    var dto = DataModelRecordMapper.MapToDTO(record);
    Assert.Equal(SampleRecordId, dto.Id);
  }

  [Fact]
  public void MapToDTO_ValidRecord_MapsModelId()
  {
    var record = BuildFullRecord();
    var dto = DataModelRecordMapper.MapToDTO(record);
    Assert.Equal(SampleModelId, dto.ModelId);
  }

  [Fact]
  public void MapToDTO_ValidRecord_MapsExternalId()
  {
    var record = BuildFullRecord();
    var dto = DataModelRecordMapper.MapToDTO(record);
    Assert.Equal("Unspecified.PersonGenderType.00000000-0000-0000-0000-000000000000", dto.ExternalId);
  }

  [Fact]
  public void MapToDTO_NullExternalId_ReturnsEmptyString()
  {
    var record = BuildFullRecord();
    record.ExternalId = null;
    var dto = DataModelRecordMapper.MapToDTO(record);
    Assert.Equal(string.Empty, dto.ExternalId);
  }

  // -------------------------------------------------------------------
  // MapToDTO — pole Fields
  // -------------------------------------------------------------------

  [Fact]
  public void MapToDTO_ValidRecord_ContainsThreeFields()
  {
    var record = BuildFullRecord();
    var dto = DataModelRecordMapper.MapToDTO(record);
    Assert.Equal(3, dto.Fields.Count);
  }

  [Fact]
  public void MapToDTO_CodeField_IsNotLocalized()
  {
    var record = BuildFullRecord();
    var dto = DataModelRecordMapper.MapToDTO(record);
    var field = GetField(dto, "Code");
    Assert.False(field.IsLocalized);
  }

  [Fact]
  public void MapToDTO_CodeField_StringValueEqualsCode()
  {
    var record = BuildFullRecord();
    var dto = DataModelRecordMapper.MapToDTO(record);
    var field = GetField(dto, "Code");
    Assert.Equal("Unspecified", field.StringValue);
  }

  [Fact]
  public void MapToDTO_NameField_IsLocalized()
  {
    var record = BuildFullRecord();
    var dto = DataModelRecordMapper.MapToDTO(record);
    var field = GetField(dto, "Name");
    Assert.True(field.IsLocalized);
  }

  [Fact]
  public void MapToDTO_NameField_MapsCzechValue()
  {
    var record = BuildFullRecord();
    var dto = DataModelRecordMapper.MapToDTO(record);
    var field = GetField(dto, "Name");
    Assert.Equal("Nezjištěno", field.CzechValue);
  }

  [Fact]
  public void MapToDTO_NameField_MapsEnglishValue()
  {
    var record = BuildFullRecord();
    var dto = DataModelRecordMapper.MapToDTO(record);
    var field = GetField(dto, "Name");
    Assert.Equal("Unspecified", field.EnglishValue);
  }

  [Fact]
  public void MapToDTO_DescriptionField_IsLocalized()
  {
    var record = BuildFullRecord();
    var dto = DataModelRecordMapper.MapToDTO(record);
    var field = GetField(dto, "Description");
    Assert.True(field.IsLocalized);
  }

  [Fact]
  public void MapToDTO_DescriptionField_MapsCzechValue()
  {
    var record = BuildFullRecord();
    var dto = DataModelRecordMapper.MapToDTO(record);
    var field = GetField(dto, "Description");
    Assert.Equal("Nezjištěno", field.CzechValue);
  }

  [Fact]
  public void MapToDTO_DescriptionField_MapsEnglishValue()
  {
    var record = BuildFullRecord();
    var dto = DataModelRecordMapper.MapToDTO(record);
    var field = GetField(dto, "Description");
    Assert.Equal("Unspecified", field.EnglishValue);
  }

  // -------------------------------------------------------------------
  // MapToDTO — volitelná pole (null vstup)
  // -------------------------------------------------------------------

  [Fact]
  public void MapToDTO_NullCode_FieldCodeAbsent()
  {
    var record = BuildFullRecord();
    record.Code = null;
    var dto = DataModelRecordMapper.MapToDTO(record);
    Assert.DoesNotContain(dto.Fields, f => f.Key == "Code");
  }

  [Fact]
  public void MapToDTO_NullName_FieldNameAbsent()
  {
    var record = BuildFullRecord();
    record.Name = null;
    var dto = DataModelRecordMapper.MapToDTO(record);
    Assert.DoesNotContain(dto.Fields, f => f.Key == "Name");
  }

  [Fact]
  public void MapToDTO_NullDescription_FieldDescriptionAbsent()
  {
    var record = BuildFullRecord();
    record.Description = null;
    var dto = DataModelRecordMapper.MapToDTO(record);
    Assert.DoesNotContain(dto.Fields, f => f.Key == "Description");
  }

  [Fact]
  public void MapToDTO_AllOptionalFieldsNull_ReturnsEmptyFields()
  {
    var record = new DataModelRecord
    {
      Id = new DataModelRecordCompositeId { ModelId = SampleModelId, RecordId = SampleRecordId },
      Code = null,
      Name = null,
      Description = null
    };
    var dto = DataModelRecordMapper.MapToDTO(record);
    Assert.Empty(dto.Fields);
  }

  [Fact]
  public void MapToDTO_MissingLocale_LocalizedValueIsNull()
  {
    var record = BuildFullRecord();
    record.Name = BuildLocalizedValue(czech: "Pouze česky", english: null);
    var dto = DataModelRecordMapper.MapToDTO(record);
    var field = GetField(dto, "Name");
    Assert.Null(field.EnglishValue);
    Assert.Equal("Pouze česky", field.CzechValue);
  }

  // -------------------------------------------------------------------
  // MapToDTOList
  // -------------------------------------------------------------------

  [Fact]
  public void MapToDTOList_NullCollection_ThrowsArgumentNullException()
  {
    Assert.Throws<ArgumentNullException>(() => DataModelRecordMapper.MapToDTOList(null!));
  }

  [Fact]
  public void MapToDTOList_EmptyCollection_ReturnsEmptyList()
  {
    var result = DataModelRecordMapper.MapToDTOList(new List<DataModelRecord>());
    Assert.Empty(result);
  }

  [Fact]
  public void MapToDTOList_TwoRecords_ReturnsTwoDTOs()
  {
    var records = new List<DataModelRecord>
    {
      BuildFullRecord(),
      BuildFullRecord(recordId: Guid.NewGuid())
    };
    var result = DataModelRecordMapper.MapToDTOList(records);
    Assert.Equal(2, result.Count);
  }

  [Fact]
  public void MapToDTOList_TwoRecords_EachDTOHasCorrectId()
  {
    var secondId = Guid.NewGuid();
    var records = new List<DataModelRecord>
    {
      BuildFullRecord(),
      BuildFullRecord(recordId: secondId)
    };
    var result = DataModelRecordMapper.MapToDTOList(records);
    Assert.Equal(SampleRecordId, result[0].Id);
    Assert.Equal(secondId, result[1].Id);
  }

  // -------------------------------------------------------------------
  // Pomocné metody
  // -------------------------------------------------------------------

  private static DataModelRecord BuildFullRecord(Guid? recordId = null) => new()
  {
    Id = new DataModelRecordCompositeId
    {
      ModelId = SampleModelId,
      RecordId = recordId ?? SampleRecordId
    },
    ExternalId = "Unspecified.PersonGenderType.00000000-0000-0000-0000-000000000000",
    SourceId = Guid.Empty,
    MandantCode = null,
    Released = true,
    UtcCreatedOn = DateTimeOffset.Parse("2023-04-14T16:44:06.259Z"),
    UtcModifiedOn = DateTimeOffset.Parse("2025-05-20T15:45:13.65Z"),
    Code = "Unspecified",
    Name = BuildLocalizedValue(czech: "Nezjištěno", english: "Unspecified"),
    Description = BuildLocalizedValue(czech: "Nezjištěno", english: "Unspecified")
  };

  private static LocalizedValue<string> BuildLocalizedValue(string? czech, string? english)
  {
    var items = new List<LocalizedValueItem<string>>();
    if (czech is not null)
      items.Add(new LocalizedValueItem<string> { Locale = "cs-CZ", Value = czech });
    if (english is not null)
      items.Add(new LocalizedValueItem<string> { Locale = "en-US", Value = english });
    return new LocalizedValue<string> { Values = items };
  }

  private static DataModelRecordFieldDTO GetField(DataModelRecordDTO dto, string key)
  {
    var field = dto.Fields.SingleOrDefault(f => f.Key == key);
    Assert.NotNull(field);
    return field;
  }
}
