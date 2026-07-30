namespace AVAIntegrationModeler.Contracts.Areas;

/// <summary>
/// Požadavek na výpis oblastí.
/// </summary>
public class AreaListRequest
{
  /// <summary>
  /// <see cref="Datasource"/>
  /// </summary>
  public Datasource Datasource { get; set; } = Datasource.Database;
}
