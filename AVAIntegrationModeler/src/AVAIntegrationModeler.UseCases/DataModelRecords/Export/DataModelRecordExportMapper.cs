using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.DataModelRecords.Export;

/// <summary>
/// Mapuje <see cref="DataModelRecordDTO"/> na <see cref="ExportRecord"/> kompatibilní s formátem ASOL AVAPlace:
/// každé pole záznamu má klíč a hodnotu s polem locale-value párů.
/// </summary>
public static class DataModelRecordExportMapper
{
  private const string CzechLocale = "cs-CZ";
  private const string EnglishLocale = "en-US";

  /// <summary>Namapuje seznam záznamů na export formát.</summary>
  public static List<ExportRecord> MapToExport(IEnumerable<DataModelRecordDTO> records) =>
    records.Select(MapToExport).ToList();

  /// <summary>Namapuje jeden záznam na export formát.</summary>
  public static ExportRecord MapToExport(DataModelRecordDTO dto) => new(
    dto.ExternalId,
    dto.Fields.Select(MapField).ToList());

  private static ExportRecordField MapField(DataModelRecordFieldDTO f)
  {
    if (f.IsLocalized)
    {
      var localized = new ExportRecordFieldValue(
        [new(EnglishLocale, f.EnglishValue), new(CzechLocale, f.CzechValue)]);
      return new ExportRecordField(f.Key, localized);
    }
    return new ExportRecordField(f.Key, f.StringValue);
  }
}
