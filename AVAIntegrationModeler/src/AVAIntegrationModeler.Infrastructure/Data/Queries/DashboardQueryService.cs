using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.Dashboard;
using AVAIntegrationModeler.UseCases.Dashboard;
using AVAIntegrationModeler.UseCases.DataModels;
using AVAIntegrationModeler.UseCases.Deployments;

namespace AVAIntegrationModeler.Infrastructure.Data.Queries;

/// <summary>
/// Implementace dotazovací služby pro souhrn dashboardu.
/// </summary>
public class DashboardQueryService(
  IDataModelQueryService dataModelQueryService,
  IDeploymentsQueryService deploymentsQueryService) : IDashboardQueryService
{
  /// <inheritdoc />
  public async Task<DashboardSummaryDTO> GetSummaryAsync(CancellationToken cancellationToken = default)
  {
    var dbModels = await dataModelQueryService.ListAsync(Datasource.Database);
    var dbCount = dbModels.Count();

    int avaPlaceCount;
    try
    {
      var avaPlaceModels = await dataModelQueryService.ListAsync(Datasource.AVAPlace);
      avaPlaceCount = avaPlaceModels.Count();
    }
    catch
    {
      avaPlaceCount = 0;
    }

    var deployments = await deploymentsQueryService.ListAsync();
    var deploymentCount = deployments.Count();

    return new DashboardSummaryDTO(dbCount, avaPlaceCount, deploymentCount);
  }
}
