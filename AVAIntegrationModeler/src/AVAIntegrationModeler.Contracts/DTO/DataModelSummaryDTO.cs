namespace AVAIntegrationModeler.Contracts.DTO;

/// <summary>
/// Lehké shrnutí datového modelu, vhodné pro zobrazení v seznamech.
/// Obsahuje pouze nejdůležitější metadata potřebná pro rychlý přehled bez plného DataModelDTO.
/// </summary>
public record DataModelSummaryDTO
{
  /// <summary>
  /// Jedinečný identifikátor datového modelu.
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Technický kód datového modelu.
  /// </summary>
  public string Code { get; init; } = string.Empty;

  /// <summary>
  /// Název datového modelu.
  /// </summary>
  public string Name { get; init; } = string.Empty;
}
