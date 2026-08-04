using Ardalis.GuardClauses;
using ASOL.Core.Localization;
using AVAIntegrationModeler.AVAPlace.Models;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.AVAPlace.Mapping;

/// <summary>
/// Statická třída pro mapování mezi <see cref="DataModelRecord"/> z AVAPlace DataService a <see cref="DataModelRecordDTO"/>.
/// </summary>
public static class DataModelRecordMapper
{
  private const string CzechLocale = "cs-CZ";
  private const string EnglishLocale = "en-US";

  /// <summary>
  /// Převede <see cref="DataModelRecord"/> na <see cref="DataModelRecordDTO"/>.
  /// </summary>
  /// <param name="record">Záznam z AVAPlace DataService.</param>
  /// <returns><see cref="DataModelRecordDTO"/>.</returns>
  public static DataModelRecordDTO MapToDTO(DataModelRecord record)
  {
    Guard.Against.Null(record, nameof(record));
    Guard.Against.Null(record.Id, nameof(record.Id));

    return new DataModelRecordDTO
    {
      Id = record.Id.RecordId,
      ModelId = record.Id.ModelId,
      ExternalId = record.ExternalId ?? string.Empty,
      Fields = BuildFields(record)
    };
  }

  /// <summary>
  /// Převede seznam <see cref="DataModelRecord"/> na seznam <see cref="DataModelRecordDTO"/>.
  /// </summary>
  /// <param name="records">Záznamy z AVAPlace DataService.</param>
  /// <returns>Seznam <see cref="DataModelRecordDTO"/>.</returns>
  public static List<DataModelRecordDTO> MapToDTOList(IEnumerable<DataModelRecord> records)
  {
    Guard.Against.Null(records, nameof(records));
    return records.Select(MapToDTO).ToList();
  }

  private static List<DataModelRecordFieldDTO> BuildFields(DataModelRecord record)
  {
    var fields = new List<DataModelRecordFieldDTO>();

    if (record.Code is not null)
    {
      fields.Add(new DataModelRecordFieldDTO
      {
        Key = nameof(record.Code),
        IsLocalized = false,
        StringValue = record.Code
      });
    }

    if (record.Name is not null)
    {
      fields.Add(new DataModelRecordFieldDTO
      {
        Key = nameof(record.Name),
        IsLocalized = true,
        CzechValue = GetLocalizedValue(record.Name, CzechLocale),
        EnglishValue = GetLocalizedValue(record.Name, EnglishLocale)
      });
    }

    if (record.Description is not null)
    {
      fields.Add(new DataModelRecordFieldDTO
      {
        Key = nameof(record.Description),
        IsLocalized = true,
        CzechValue = GetLocalizedValue(record.Description, CzechLocale),
        EnglishValue = GetLocalizedValue(record.Description, EnglishLocale)
      });
    }

    return fields;
  }

  private static string? GetLocalizedValue(LocalizedValue<string> localizedValue, string locale)
  {
    return localizedValue.Values?
        .FirstOrDefault(item => item.Locale == locale)?
        .Value;
  }
}
