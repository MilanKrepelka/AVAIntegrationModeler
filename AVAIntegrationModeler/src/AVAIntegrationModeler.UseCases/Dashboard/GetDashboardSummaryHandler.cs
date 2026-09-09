using AVAIntegrationModeler.Contracts.Dashboard;

namespace AVAIntegrationModeler.UseCases.Dashboard;

/// <summary>
/// Handler pro dotaz <see cref="GetDashboardSummaryQuery"/>.
/// </summary>
public class GetDashboardSummaryHandler(IDashboardQueryService queryService)
  : IQueryHandler<GetDashboardSummaryQuery, Result<DashboardSummaryDTO>>
{
  /// <inheritdoc />
  public async Task<Result<DashboardSummaryDTO>> Handle(
    GetDashboardSummaryQuery request, CancellationToken cancellationToken)
  {
    var summary = await queryService.GetSummaryAsync(cancellationToken);
    return Result<DashboardSummaryDTO>.Success(summary);
  }
}
