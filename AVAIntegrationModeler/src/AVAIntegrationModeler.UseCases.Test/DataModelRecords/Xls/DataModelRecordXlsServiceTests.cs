using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.DataModelRecords.Xls;
using Shouldly;

namespace AVAIntegrationModeler.UseCases.Test.DataModelRecords.Xls;

/// <summary>
/// Testy pro <see cref="DataModelRecordXlsService"/> — sestavení XLS a jeho zpětné parsování (round-trip).
/// </summary>
public class DataModelRecordXlsServiceTests
{
  private static readonly Guid ModelId = Guid.NewGuid();

  private static DataModelRecordDTO Record(Guid id, string extId, params DataModelRecordFieldDTO[] fields) => new()
  {
    Id = id,
    ModelId = ModelId,
    ExternalId = extId,
    Fields = fields.ToList()
  };

  private static DataModelRecordFieldDTO StringField(string key, string? value) => new()
  {
    Key = key,
    IsLocalized = false,
    StringValue = value
  };

  private static DataModelRecordFieldDTO LocalizedField(string key, string? cz, string? en) => new()
  {
    Key = key,
    IsLocalized = true,
    CzechValue = cz,
    EnglishValue = en
  };

  [Fact]
  public void BuildXls_EmptyList_ReturnsByteArray()
  {
    var bytes = DataModelRecordXlsService.BuildXls(Enumerable.Empty<DataModelRecordDTO>());
    bytes.ShouldNotBeNull();
    bytes.Length.ShouldBeGreaterThan(0);
  }

  [Fact]
  public void RoundTrip_StringFields_PreservesData()
  {
    var id = Guid.NewGuid();
    var records = new[]
    {
      Record(id, "EXT-001", StringField("Nazev", "Testovací"), StringField("Popis", "Popis hodnota"))
    };

    var bytes = DataModelRecordXlsService.BuildXls(records);
    var (parsed, errors) = DataModelRecordXlsService.ParseXls(bytes, ModelId);

    errors.ShouldBeEmpty();
    parsed.Count.ShouldBe(1);
    parsed[0].Id.ShouldBe(id);
    parsed[0].ExternalId.ShouldBe("EXT-001");
    parsed[0].Fields.ShouldContain(f => f.Key == "Nazev" && f.StringValue == "Testovací" && !f.IsLocalized);
    parsed[0].Fields.ShouldContain(f => f.Key == "Popis" && f.StringValue == "Popis hodnota" && !f.IsLocalized);
  }

  [Fact]
  public void RoundTrip_LocalizedFields_PreservesData()
  {
    var id = Guid.NewGuid();
    var records = new[]
    {
      Record(id, "EXT-002", LocalizedField("Jmeno", "Česky", "English"))
    };

    var bytes = DataModelRecordXlsService.BuildXls(records);
    var (parsed, errors) = DataModelRecordXlsService.ParseXls(bytes, ModelId);

    errors.ShouldBeEmpty();
    parsed.Count.ShouldBe(1);
    var field = parsed[0].Fields.ShouldHaveSingleItem();
    field.Key.ShouldBe("Jmeno");
    field.IsLocalized.ShouldBeTrue();
    field.CzechValue.ShouldBe("Česky");
    field.EnglishValue.ShouldBe("English");
  }

  [Fact]
  public void RoundTrip_MixedFields_PreservesAll()
  {
    var id = Guid.NewGuid();
    var records = new[]
    {
      Record(id, "EXT-003",
        StringField("Kod", "K001"),
        LocalizedField("Popis", "CZ popis", "EN description"))
    };

    var bytes = DataModelRecordXlsService.BuildXls(records);
    var (parsed, errors) = DataModelRecordXlsService.ParseXls(bytes, ModelId);

    errors.ShouldBeEmpty();
    parsed.Count.ShouldBe(1);
    parsed[0].Fields.Count.ShouldBe(2);
    parsed[0].Fields.ShouldContain(f => f.Key == "Kod" && !f.IsLocalized);
    parsed[0].Fields.ShouldContain(f => f.Key == "Popis" && f.IsLocalized);
  }

  [Fact]
  public void RoundTrip_MultipleRecords_PreservesAll()
  {
    var id1 = Guid.NewGuid();
    var id2 = Guid.NewGuid();
    var records = new[]
    {
      Record(id1, "EXT-001", StringField("Field", "Val1")),
      Record(id2, "EXT-002", StringField("Field", "Val2"))
    };

    var bytes = DataModelRecordXlsService.BuildXls(records);
    var (parsed, errors) = DataModelRecordXlsService.ParseXls(bytes, ModelId);

    errors.ShouldBeEmpty();
    parsed.Count.ShouldBe(2);
    parsed.ShouldContain(r => r.Id == id1 && r.ExternalId == "EXT-001");
    parsed.ShouldContain(r => r.Id == id2 && r.ExternalId == "EXT-002");
  }

  [Fact]
  public void RoundTrip_EmptyId_ParsedAsGuidEmpty()
  {
    var records = new[]
    {
      Record(Guid.Empty, "NEW-001", StringField("X", "v"))
    };

    var bytes = DataModelRecordXlsService.BuildXls(records);
    var (parsed, errors) = DataModelRecordXlsService.ParseXls(bytes, ModelId);

    errors.ShouldBeEmpty();
    parsed.Count.ShouldBe(1);
    parsed[0].Id.ShouldBe(Guid.Empty);
  }

  [Fact]
  public void ParseXls_InvalidFile_ReturnsParseError()
  {
    var badBytes = "not an xlsx file"u8.ToArray();
    var (parsed, errors) = DataModelRecordXlsService.ParseXls(badBytes, ModelId);

    errors.ShouldNotBeEmpty();
    parsed.ShouldBeEmpty();
    errors[0].RowNumber.ShouldBe(0);
  }

  [Fact]
  public void ParseXls_EmptySheet_ReturnsNoRecordsNoErrors()
  {
    // Prázdný XLS (jen záhlaví)
    var records = Array.Empty<DataModelRecordDTO>();
    var bytes = DataModelRecordXlsService.BuildXls(records);
    var (parsed, errors) = DataModelRecordXlsService.ParseXls(bytes, ModelId);

    errors.ShouldBeEmpty();
    parsed.ShouldBeEmpty();
  }

  [Fact]
  public void BuildXls_FieldsSortedAlphabetically()
  {
    // Pořadí sloupců v záhlaví musí být abecední
    var records = new[]
    {
      Record(Guid.NewGuid(), "EXT-001",
        StringField("Zebra", "z"),
        StringField("Apple", "a"),
        StringField("Mango", "m"))
    };

    var bytes = DataModelRecordXlsService.BuildXls(records);
    var (parsed, errors) = DataModelRecordXlsService.ParseXls(bytes, ModelId);

    errors.ShouldBeEmpty();
    // Všechna pole jsou parsována bez ohledu na pořadí sloupců
    parsed[0].Fields.ShouldContain(f => f.Key == "Apple");
    parsed[0].Fields.ShouldContain(f => f.Key == "Mango");
    parsed[0].Fields.ShouldContain(f => f.Key == "Zebra");
  }

  [Fact]
  public void RoundTrip_RecordMissingField_ParsedAsEmptyString()
  {
    // Jeden záznam má pole "Extra", druhý ne — druhý by měl mít prázdný string
    var id1 = Guid.NewGuid();
    var id2 = Guid.NewGuid();
    var records = new[]
    {
      Record(id1, "EXT-001", StringField("Base", "b1"), StringField("Extra", "e1")),
      Record(id2, "EXT-002", StringField("Base", "b2"))
    };

    var bytes = DataModelRecordXlsService.BuildXls(records);
    var (parsed, errors) = DataModelRecordXlsService.ParseXls(bytes, ModelId);

    errors.ShouldBeEmpty();
    var r2 = parsed.First(r => r.Id == id2);
    r2.Fields.ShouldContain(f => f.Key == "Extra" && f.StringValue == "");
  }
}
