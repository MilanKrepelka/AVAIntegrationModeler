namespace AVAIntegrationModeler.Contracts.DTO;

public record DataModelRecordDTO
{
  [System.Text.Json.Serialization.JsonPropertyName("recordId")]
  public Guid Id { get; init; }

  public Guid ModelId { get; init; }

  public string ExternalId { get; init; } = string.Empty;

  public List<DataModelRecordFieldDTO> Fields { get; init; } = new();

  /// <summary>
  /// Datum a čas vytvoření záznamu datového modelu v databázi (UTC).
  /// </summary>
  public DateTime? CreatedAt { get; init; }

  /// <summary>
  /// Datum a čas posledního uložení záznamu datového modelu do databáze (UTC).
  /// </summary>
  public DateTime? LastSavedAt { get; init; }
}
