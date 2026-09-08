namespace AVAIntegrationModeler.UseCases.DataModelRecords.Export;

/// <summary>Export reprezentace záznamu DataModelu ve formátu kompatibilním s ASOL AVAPlace.</summary>
public record ExportRecord(string ExternalId, List<ExportRecordField> Fields);

/// <summary>
/// Export pole záznamu. <see cref="Value"/> je při serializaci buď prostý string (nelokalizované pole),
/// nebo objekt <see cref="ExportRecordFieldValue"/> s polem locale-value párů (lokalizované pole).
/// </summary>
public record ExportRecordField(string Key, object? Value);

/// <summary>Kontejner hodnot pole záznamu s volitelnou lokalizací.</summary>
public record ExportRecordFieldValue(List<ExportRecordLocaleValue> Values);

/// <summary>
/// Jedna hodnota pole záznamu. <see cref="Locale"/> je null pro nelokalizovaná pole (null bude
/// vynecháno v JSON díky <c>SkipEmptyCollections</c> modifieru v <c>ExportJsonOptions</c>).
/// </summary>
public record ExportRecordLocaleValue(string? Locale, string? Value);
