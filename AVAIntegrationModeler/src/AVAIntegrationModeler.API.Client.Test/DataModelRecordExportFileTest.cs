using System.Text.Json;
using AVAIntegrationModeler.API.Serialization;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.DataModelRecords.Export;

namespace AVAIntegrationModeler.API.Client.Test;

/// <summary>
/// Explorační test — mapuje ukázkový <see cref="DataModelRecordDTO"/> na export formát
/// a zapíše výsledný JSON soubor do LocalApplicationData pro ruční inspekci.
/// </summary>
public class DataModelRecordExportFileTest
{
  private static readonly Guid ModelId = Guid.Parse("11111111-0000-0000-0000-000000000001");
  private static readonly Guid RecordId = Guid.Parse("22222222-0000-0000-0000-000000000001");

  private static DataModelRecordDTO BuildSampleRecord() => new()
  {
    Id = RecordId,
    ModelId = ModelId,
    ExternalId = "ForEnjoy.TestModelTypeByKRE.00000000-0000-0000-0000-000000000000",
    Fields =
    [
      new DataModelRecordFieldDTO
      {
        Key = "Code",
        IsLocalized = false,
        StringValue = "ForEnjoy"
      },
      new DataModelRecordFieldDTO
      {
        Key = "Description",
        IsLocalized = false,
        StringValue = "Bla bla pro radost"
      },
      new DataModelRecordFieldDTO
      {
        Key = "Name",
        IsLocalized = true,
        EnglishValue = "For enjoy",
        CzechValue = "Pro radost"
      }
    ]
  };

  /// <summary>
  /// Mapuje ukázkový záznam a zapíše JSON soubor do LocalApplicationData.
  /// Cesta je vypsána do konzole — otevři ji pro inspekci výstupu.
  /// </summary>
  [Fact]
  public async Task ExportRecord_WritesJsonFile_ForManualInspection()
  {
    var dto = BuildSampleRecord();

    var exportRecord = DataModelRecordExportMapper.MapToExport(dto);
    var json = JsonSerializer.Serialize(exportRecord, ExportJsonOptions.Instance);

    var outPath = Path.Combine(
      Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
      "datamodelrecord-export-sample.json");

    await File.WriteAllTextAsync(outPath, json);

    Console.WriteLine($"Výstupní soubor: {outPath}");
    Console.WriteLine();
    Console.WriteLine(json);

    Assert.True(File.Exists(outPath));
  }

  /// <summary>
  /// Mapuje seznam dvou ukázkových záznamů (jako je to uloženo v ZIP) a zapíše JSON soubor.
  /// </summary>
  [Fact]
  public async Task ExportRecordList_WritesJsonFile_ForManualInspection()
  {
    var records = new List<DataModelRecordDTO>
    {
      BuildSampleRecord(),
      BuildSampleRecord() with
      {
        Id = Guid.Parse("22222222-0000-0000-0000-000000000002"),
        ExternalId = "ForEnjoy.TestModelTypeByKRE.00000000-0000-0000-0000-000000000002",
        Fields =
        [
          new DataModelRecordFieldDTO { Key = "Code", IsLocalized = false, StringValue = "ForWork" },
          new DataModelRecordFieldDTO { Key = "Name", IsLocalized = true, EnglishValue = "For work", CzechValue = "Pro práci" }
        ]
      }
    };

    var exportRecords = DataModelRecordExportMapper.MapToExport(records);
    var json = JsonSerializer.Serialize(exportRecords, ExportJsonOptions.Instance);

    var outPath = Path.Combine(
      Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
      "datamodelrecords-export-list.json");

    await File.WriteAllTextAsync(outPath, json);

    Console.WriteLine($"Výstupní soubor: {outPath}");
    Console.WriteLine();
    Console.WriteLine(json);

    Assert.True(File.Exists(outPath));
  }
}
