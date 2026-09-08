namespace AVAIntegrationModeler.Contracts.DTO;

/// <summary>
/// DTO pro MapLayout — diagram pojmenované mapy.
/// </summary>
public record MapLayoutDTO
{
  public string Key { get; init; } = string.Empty;
  public string? DiagramJson { get; init; }
  public DateTime? LastMapSave { get; init; }
}
