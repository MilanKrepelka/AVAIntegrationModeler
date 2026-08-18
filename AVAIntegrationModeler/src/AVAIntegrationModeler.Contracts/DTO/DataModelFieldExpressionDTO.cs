namespace AVAIntegrationModeler.Contracts.DTO;

/// <summary>
/// DTO pro definici výrazu (expression) počítaného pole datového modelu.
/// </summary>
public record DataModelFieldExpressionDTO
{
  /// <summary>
  /// Text výrazu.
  /// </summary>
  public string Value { get; init; } = string.Empty;

  /// <summary>
  /// Pořadí vyhodnocení výrazu.
  /// </summary>
  public int Order { get; init; }
}
