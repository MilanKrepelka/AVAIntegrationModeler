using AVAIntegrationModeler.Contracts.Dashboard;
using AVAIntegrationModeler.UseCases.Dashboard;

namespace AVAIntegrationModeler.API.Dashboard;

/// <summary>
/// Vrátí souhrn statistik dashboardu.
/// </summary>
public class GetDashboardSummaryEndpoint(IMediator mediator)
  : EndpointWithoutRequest<GetDashboardSummaryResponse>
{
  /// <inheritdoc />
  public override void Configure()
  {
    Get("/Dashboard/summary");
    AllowAnonymous();
    Summary(s => s.Summary = "Vrátí souhrn statistik dashboardu (počty modelů a nasazení).");
  }

  /// <inheritdoc />
  public override async Task HandleAsync(CancellationToken ct)
  {
    var result = await mediator.Send(new GetDashboardSummaryQuery(), ct);
    if (result.IsSuccess)
      Response = new GetDashboardSummaryResponse { Summary = result.Value };
  }
}
