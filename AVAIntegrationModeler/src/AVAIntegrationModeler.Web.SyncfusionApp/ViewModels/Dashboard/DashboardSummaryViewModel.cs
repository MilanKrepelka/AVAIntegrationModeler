namespace AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.Dashboard;

/// <summary>
/// View model pro souhrn dashboardu na hlavní stránce.
/// </summary>
public class DashboardSummaryViewModel
{
  /// <summary>Počet datových modelů v lokální databázi.</summary>
  public int DataModelsDatabase { get; init; }

  /// <summary>Počet datových modelů v AVAPlace.</summary>
  public int DataModelsAvaPlace { get; init; }

  /// <summary>Celkový počet nasazení.</summary>
  public int DeploymentCount { get; init; }
}
