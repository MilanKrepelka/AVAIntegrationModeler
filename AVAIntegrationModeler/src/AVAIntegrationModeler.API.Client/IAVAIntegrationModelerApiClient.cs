using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Contracts.Scenarios;

namespace AVAIntegrationModeler.API.Client;

/// <summary>
/// Klient pro AVA Integration Modeler API.
/// </summary>
public interface IAVAIntegrationModelerApiClient
{
  /// <summary>
  /// Vrátí všechny datové modely ze zadaného datového zdroje.
  /// </summary>
  /// <param name="datasource">Datový zdroj.</param>
  /// <param name="cancellationToken">Token pro zrušení operace.</param>
  /// <returns>Seznam datových modelů.</returns>
  public Task<DataModelListResponse> GetDataModels(Datasource datasource, CancellationToken cancellationToken);

  /// <summary>
  /// Vrátí všechny Integrační scénáře ze zadaného datového zdroje.
  /// </summary>
  /// <param name="datasource">Datový zdroj.</param>
  /// <param name="cancellationToken">Token pro zrušení operace.</param>
  /// <returns>Seznam integračních scénářů.</returns>
  public Task<ScenarioListResponse> GetScenarios(Datasource datasource, CancellationToken cancellationToken);

}
