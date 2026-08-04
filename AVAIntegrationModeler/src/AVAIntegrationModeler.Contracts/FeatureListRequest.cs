using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.API.Features;

/// <summary>
/// Požadavek na výpis featur
/// </summary>
public class FeatureListRequest
{
  /// <summary>
  /// <see cref="AVAIntegrationModeler.Contracts.Datasource"/>
  /// </summary>
  public Datasource Datasource { get; set; } = Datasource.Database;
}
