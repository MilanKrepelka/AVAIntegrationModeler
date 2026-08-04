using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.API.IntegrationMaps;

/// <summary>
/// Požadavek na výpis featur
/// </summary>
public class IntegrationMapListRequest
{
  /// <summary>
  /// <see cref="Contracts.Datasource"/>
  /// </summary>
  public Datasource Datasource { get; set; } = Datasource.Database;
}
