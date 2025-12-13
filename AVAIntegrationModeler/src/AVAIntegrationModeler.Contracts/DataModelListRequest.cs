using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.Contracts;

/// <summary>
/// Požadavek na výpis modelů dat
/// </summary>
public class DataModelListRequest
{
  /// <summary>
  /// <see cref="AVAIntegrationModeler.Contracts.Datasource"/>
  /// </summary>
  public Datasource Datasource { get; set; } = Datasource.Database;
}
