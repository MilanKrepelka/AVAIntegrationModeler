namespace AVAIntegrationModeler.Contracts.DTO;

public record DataModelRecordDTO
{
  [System.Text.Json.Serialization.JsonPropertyName("recordId")]
  public Guid Id { get; init; }

  public Guid ModelId { get; init; }

  public string ExternalId { get; init; } = string.Empty;

  public List<DataModelRecordFieldDTO> Fields { get; init; } = new();
}
