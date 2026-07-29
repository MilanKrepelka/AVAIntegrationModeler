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
  Task<Result<ScenarioDTO>> UpdateScenario(Datasource datasource, ScenarioDTO scenario, CancellationToken cancellationToken);

  /// <summary>
  /// Smaže integrační scénář ve zadaném datovém zdroji a vrátí výsledek jako <see cref="Result{T}"/>.
  /// </summary>
  /// <param name="datasource"><see cref="Datasource"/></param>
  /// <param name="scenarioCode">Kód integrační scénář</param>
  /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
  /// <returns>Výsledek operace obsahující aktualizovaný scénář.</returns>
  Task<Result> DeleteScenario(Datasource datasource, string scenarioCode, CancellationToken cancellationToken);

  Task<DataModelDTO> GetDataModel(Datasource datasource, Guid dataModelId, CancellationToken cancellationToken);
  Task<Result<Guid>> CreateDataModel(Datasource datasource, DataModelDTO dataModel, CancellationToken cancellationToken);
  Task<Result<DataModelDTO>> UpdateDataModel(Datasource datasource, DataModelDTO dataModel, CancellationToken cancellationToken);
  Task<Result> DeleteDataModel(Datasource datasource, Guid dataModelId, CancellationToken cancellationToken);

  Task<DataModelRecordListResponse> GetDataModelRecords(Datasource datasource, Guid? modelId, CancellationToken cancellationToken);
  Task<DataModelRecordDTO> GetDataModelRecord(Datasource datasource, Guid recordId, CancellationToken cancellationToken);
  Task<Result<Guid>> CreateDataModelRecord(Datasource datasource, DataModelRecordDTO record, CancellationToken cancellationToken);
  Task<Result<Guid>> UpdateDataModelRecord(Datasource datasource, DataModelRecordDTO record, CancellationToken cancellationToken);
  Task<Result> DeleteDataModelRecord(Datasource datasource, Guid recordId, CancellationToken cancellationToken);
  Task<byte[]> ExportDataModelRecords(Datasource datasource, List<Guid> recordIds, CancellationToken cancellationToken);

  /// <summary>
  /// Importuje datový model z AVAPlace do lokální databáze.
  /// Pokud model se stejným kódem již existuje, provede aktualizaci (upsert).
  /// </summary>
  /// <param name="avaPlaceModelId">Identifikátor datového modelu v AVAPlace.</param>
  /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
  /// <returns>Identifikátor importovaného modelu v lokální databázi.</returns>
  Task<Result<Guid>> ImportDataModelFromAvaPlace(Guid avaPlaceModelId, CancellationToken cancellationToken);
}
