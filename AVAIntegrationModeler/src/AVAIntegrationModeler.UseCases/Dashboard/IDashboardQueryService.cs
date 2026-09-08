using AVAIntegrationModeler.Contracts.Dashboard;

namespace AVAIntegrationModeler.UseCases.Dashboard;

/// <summary>
/// Dotazovací služba pro souhrn dashboardu.
/// </summary>
public interface IDashboardQueryService
{
  /// <summary>
  /// Vrátí souhrn statistik dashboardu.
  /// </summary>
  Task<DashboardSummaryDTO> GetSummaryAsync(CancellationToken cancellationToken = default);
}
