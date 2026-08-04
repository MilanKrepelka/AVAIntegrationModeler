namespace AVAIntegrationModeler.Contracts.DTO;

/// <summary>
/// DTO pro nasazení (Deployment).
/// </summary>
public record DeploymentDTO
{
  /// <summary>
  /// Jedinečný identifikátor nasazení.
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Kód nasazení pro jednoznačnou identifikaci.
  /// </summary>
  public string Code { get; init; } = string.Empty;

  /// <summary>
  /// Název nasazení.
  /// </summary>
  public string Name { get; init; } = string.Empty;

  /// <summary>
  /// URL odkaz na ticket nasazení (volitelné).
  /// </summary>
  public string? Ticket { get; init; }

  /// <summary>
  /// Popis nasazení (volitelné).
  /// </summary>
  public string? Description { get; init; }

  /// <summary>
  /// Identifikátory datových modelů zahrnutých v nasazení.
  /// </summary>
  public List<Guid> DataModelIds { get; init; } = new();
}
