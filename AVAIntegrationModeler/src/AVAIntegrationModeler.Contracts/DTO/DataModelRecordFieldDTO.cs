namespace AVAIntegrationModeler.Contracts.DTO;

public record DataModelRecordFieldDTO
{
  public string Key { get; init; } = string.Empty;

  public bool IsLocalized { get; init; }

  public string? StringValue { get; init; }

  public string? CzechValue { get; init; }

  public string? EnglishValue { get; init; }
}
