using AVAIntegrationModeler.Contracts.Dashboard;

namespace AVAIntegrationModeler.UseCases.Dashboard;

/// <summary>
/// Dotaz pro získání souhrnu dashboardu.
/// </summary>
public record GetDashboardSummaryQuery() : IQuery<Result<DashboardSummaryDTO>>;
