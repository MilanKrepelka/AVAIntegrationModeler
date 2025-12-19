using Ardalis.Result;
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

  /// <summary>
  /// Vrátí všechny Integrační scénáře ze zadaného datového zdroje.
  /// </summary>
  /// <param name="datasource">Datový zdroj.</param>
  /// <param name="scenarioId">ID scénáře.</param>
  /// <param name="cancellationToken">Token pro zrušení operace.</param>
  /// <returns><see cref="ScenarioDTO"/></returns>
  public Task<ScenarioDTO> GetScenario(Datasource datasource, Guid scenarioId, CancellationToken cancellationToken);

  /// <summary>
  /// Vrátí všechny Integrační scénáře ze zadaného datového zdroje.
  /// </summary>
  /// <param name="datasource">Datový zdroj.</param>
  /// <param name="scenarioCode">Kód scénáře.</param>
  /// <param name="cancellationToken">Token pro zrušení operace.</param>
  /// <returns><see cref="ScenarioDTO"/></returns>
  public Task<ScenarioDTO> GetScenario(Datasource datasource, string scenarioCode, CancellationToken cancellationToken);

  /// <summary>
  /// Vrátí všechny Integrační features ze zadaného datového zdroje.
  /// </summary>
  /// <param name="datasource">Datový zdroj.</param>
  /// <param name="cancellationToken">Token pro zrušení operace.</param>
  /// <returns>Seznam integračních features.</returns>
  public Task<FeatureListResponse> GetFeatures(Datasource datasource, CancellationToken cancellationToken);

  /// <summary>
  /// Aktualizuje integrační scénář ve zadaném datovém zdroji.
  /// </summary>
  /// <param name="datasource"><see cref="Datasource"/></param>
  /// <param name="scenario">Integrační scénář</param>
  /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
  /// <returns>Integrační scénář</returns>
  Task<ScenarioDTO> UpdateScenario(Datasource datasource, ScenarioDTO scenario, CancellationToken cancellationToken);

  /// <summary>
  /// Vytvoří nový integrační scénář ve zadaném datovém zdroji a vrátí výsledek jako <see cref="Result{T}"/>.
  /// </summary>
  /// <param name="datasource"><see cref="Datasource"/></param>
  /// <param name="scenario">Integrační scénář</param>
  /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
  /// <returns>Výsledek operace obsahující ID nového scénáře.</returns>
  Task<Result<Guid>> CreateScenario(Datasource datasource, ScenarioDTO scenario, CancellationToken cancellationToken);

  /// <summary>
  /// Aktualizuje integrační scénář ve zadaném datovém zdroji a vrátí výsledek jako <see cref="Result{T}"/>.
  /// </summary>
  /// <param name="datasource"><see cref="Datasource"/></param>
  /// <param name="scenario">Integrační scénář</param>
  /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
  /// <returns>Výsledek operace obsahující aktualizovaný scénář.</returns>
  Task<Result<ScenarioDTO>> UpdateScenarioResult(Datasource datasource, ScenarioDTO scenario, CancellationToken cancellationToken);
}
