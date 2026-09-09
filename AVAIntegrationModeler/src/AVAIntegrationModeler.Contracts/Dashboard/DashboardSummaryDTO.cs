namespace AVAIntegrationModeler.Contracts.Dashboard;

/// <summary>
/// Souhrn statistik pro dashboard.
/// </summary>
public record DashboardSummaryDTO(
  /// <summary>Počet datových modelů v lokální databázi.</summary>
  int DataModelsDatabase,
  /// <summary>Počet datových modelů v AVAPlace.</summary>
  int DataModelsAvaPlace,
  /// <summary>Celkový počet nasazení.</summary>
  int DeploymentCount
);
