namespace AVAIntegrationModeler.Contracts.Dashboard;

/// <summary>
/// Odpověď endpointu pro souhrn dashboardu.
/// </summary>
public class GetDashboardSummaryResponse
{
  /// <summary>
  /// Souhrn statistik dashboardu.
  /// </summary>
  public DashboardSummaryDTO Summary { get; set; } = default!;
}
