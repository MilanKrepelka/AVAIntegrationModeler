namespace AVAIntegrationModeler.Contracts.Deployments;

/// <summary>
/// Požadavek pro načtení posledních nasazení.
/// </summary>
public class GetRecentDeploymentsRequest
{
  /// <summary>
  /// Počet nasazení k vrácení. Výchozí hodnota je 5.
  /// </summary>
  public int Count { get; set; } = 5;
}
