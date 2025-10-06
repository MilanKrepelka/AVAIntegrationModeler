using AVAIntegrationModeler.API.Scenarios.ViewModels;
using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.API.Scenarios;

/// <summary>
/// 
/// </summary>
public class ScenarioListFullPreviewRequest
{
  public Datasource Datasource { get; set; } = Datasource.Database;
}
