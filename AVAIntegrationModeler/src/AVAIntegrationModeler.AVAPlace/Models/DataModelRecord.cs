using ASOL.Core.Localization;

namespace AVAIntegrationModeler.AVAPlace.Models;

/// <summary>
/// Záznam datového modelu načtený z AVAPlace DataService (unified data).
/// </summary>
public sealed class DataModelRecord
{
  /// <summary>
  /// Složený identifikátor záznamu (ModelId + RecordId).
  /// </summary>
  public DataModelRecordCompositeId Id { get; set; } = new();

  /// <summary>
  /// Externí identifikátor záznamu.
  /// </summary>
  public string? ExternalId { get; set; }

  /// <summary>
  /// Identifikátor zdrojového záznamu.
  /// </summary>
  public Guid SourceId { get; set; }

  /// <summary>
  /// Kód mandanta; null pokud není přiřazen.
  /// </summary>
  public string? MandantCode { get; set; }

  /// <summary>
  /// Příznak zveřejnění záznamu.
  /// </summary>
  public bool Released { get; set; }

  /// <summary>
  /// Datum a čas vytvoření záznamu (UTC).
  /// </summary>
  public DateTimeOffset UtcCreatedOn { get; set; }

  /// <summary>
  /// Datum a čas poslední úpravy záznamu (UTC).
  /// </summary>
  public DateTimeOffset UtcModifiedOn { get; set; }

  /// <summary>
  /// Kód záznamu.
  /// </summary>
  public string? Code { get; set; }

  /// <summary>
  /// Lokalizovaný název záznamu.
  /// </summary>
  public LocalizedValue<string>? Name { get; set; }

  /// <summary>
  /// Lokalizovaný popis záznamu.
  /// </summary>
  public LocalizedValue<string>? Description { get; set; }
}
