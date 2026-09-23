using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.DataModelRecords.Export;
using Shouldly;

namespace AVAIntegrationModeler.UseCases.Test.DataModelRecords.Export;

/// <summary>
/// Unit testy pro <see cref="DataModelRecordExportMapper"/>.
/// </summary>
public class DataModelRecordExportMapperTests
{
  private static readonly Guid ModelId = Guid.NewGuid();
  private static readonly Guid RecordId = Guid.NewGuid();

  private static DataModelRecordDTO SimpleRecord(string externalId = "EXT-001") => new()
  {
    Id = RecordId,
    ModelId = ModelId,
    ExternalId = externalId,
    Fields = []
  };

  // --- ModelId a RecordId ---

  [Fact]
  public void MapToExport_SetsModelId()
  {
    var result = DataModelRecordExportMapper.MapToExport(SimpleRecord());

    result.ModelId.ShouldBe(ModelId);
  }

  [Fact]
  public void MapToExport_SetsRecordId()
  {
    var result = DataModelRecordExportMapper.MapToExport(SimpleRecord());

    result.RecordId.ShouldBe(RecordId);
  }

  [Fact]
  public void MapToExport_SetsExternalId()
  {
    var result = DataModelRecordExportMapper.MapToExport(SimpleRecord("MY.EXT.ID"));

    result.ExternalId.ShouldBe("MY.EXT.ID");
  }

  // --- Nelokalizované pole ---

  [Fact]
  public void MapToExport_StringField_MapsKeyAndStringValue()
  {
    var dto = SimpleRecord() with
    {
      Fields =
      [
        new DataModelRecordFieldDTO { Key = "Code", IsLocalized = false, StringValue = "ABC" }
      ]
    };

    var result = DataModelRecordExportMapper.MapToExport(dto);

    var field = result.Fields.ShouldHaveSingleItem();
    field.Key.ShouldBe("Code");
    field.Value.ShouldBe("ABC");
  }

  [Fact]
  public void MapToExport_StringField_NullValue_MapsAsNull()
  {
    var dto = SimpleRecord() with
    {
      Fields = [new DataModelRecordFieldDTO { Key = "Opt", IsLocalized = false, StringValue = null }]
    };

    var result = DataModelRecordExportMapper.MapToExport(dto);

    result.Fields.ShouldHaveSingleItem().Value.ShouldBeNull();
  }

  // --- Lokalizované pole ---

  [Fact]
  public void MapToExport_LocalizedField_ValueIsExportRecordFieldValue()
  {
    var dto = SimpleRecord() with
    {
      Fields =
      [
        new DataModelRecordFieldDTO
        {
          Key = "Name",
          IsLocalized = true,
          EnglishValue = "For Enjoy",
          CzechValue = "Pro radost"
        }
      ]
    };

    var result = DataModelRecordExportMapper.MapToExport(dto);

    var field = result.Fields.ShouldHaveSingleItem();
    field.Key.ShouldBe("Name");
    field.Value.ShouldBeOfType<ExportRecordFieldValue>();
  }

  [Fact]
  public void MapToExport_LocalizedField_ContainsEnglishLocale()
  {
    var dto = SimpleRecord() with
    {
      Fields =
      [
        new DataModelRecordFieldDTO { Key = "Name", IsLocalized = true, EnglishValue = "For Enjoy", CzechValue = "Pro radost" }
      ]
    };

    var result = DataModelRecordExportMapper.MapToExport(dto);

    var localized = (ExportRecordFieldValue)result.Fields[0].Value!;
    localized.Values.ShouldContain(v => v.Locale == "en-US" && v.Value == "For Enjoy");
  }

  [Fact]
  public void MapToExport_LocalizedField_ContainsCzechLocale()
  {
    var dto = SimpleRecord() with
    {
      Fields =
      [
        new DataModelRecordFieldDTO { Key = "Name", IsLocalized = true, EnglishValue = "For Enjoy", CzechValue = "Pro radost" }
      ]
    };

    var result = DataModelRecordExportMapper.MapToExport(dto);

    var localized = (ExportRecordFieldValue)result.Fields[0].Value!;
    localized.Values.ShouldContain(v => v.Locale == "cs-CZ" && v.Value == "Pro radost");
  }

  // --- Více polí a zachování pořadí ---

  [Fact]
  public void MapToExport_MultipleFields_PreservesOrder()
  {
    var dto = SimpleRecord() with
    {
      Fields =
      [
        new DataModelRecordFieldDTO { Key = "Code", IsLocalized = false, StringValue = "A" },
        new DataModelRecordFieldDTO { Key = "Name", IsLocalized = true, EnglishValue = "E", CzechValue = "C" },
        new DataModelRecordFieldDTO { Key = "Description", IsLocalized = false, StringValue = "D" }
      ]
    };

    var result = DataModelRecordExportMapper.MapToExport(dto);

    result.Fields.Select(f => f.Key).ShouldBe(["Code", "Name", "Description"]);
  }

  // --- Přetížení pro seznam ---

  [Fact]
  public void MapToExport_List_MapsAllRecords()
  {
    var id1 = Guid.NewGuid();
    var id2 = Guid.NewGuid();
    var records = new List<DataModelRecordDTO>
    {
      new() { Id = id1, ModelId = ModelId, ExternalId = "EXT-1", Fields = [] },
      new() { Id = id2, ModelId = ModelId, ExternalId = "EXT-2", Fields = [] }
    };

    var result = DataModelRecordExportMapper.MapToExport(records);

    result.Count.ShouldBe(2);
    result[0].RecordId.ShouldBe(id1);
    result[1].RecordId.ShouldBe(id2);
  }

  [Fact]
  public void MapToExport_EmptyList_ReturnsEmptyList()
  {
    var result = DataModelRecordExportMapper.MapToExport([]);

    result.ShouldBeEmpty();
  }
}
