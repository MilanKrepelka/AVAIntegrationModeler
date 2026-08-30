using Ardalis.Result;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.Dashboard;
using AVAIntegrationModeler.Contracts.DataModels;
using AVAIntegrationModeler.Contracts.Deployments;
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
  Task<byte[]> ExportDataModels(Datasource datasource, List<Guid> modelIds, CancellationToken cancellationToken);

  /// <summary>
  /// Smaže všechny datové modely z lokální databáze včetně jejich záznamů (DataModelRecord).
  /// </summary>
  /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
  /// <returns>Souhrn počtu smazaných modelů a záznamů.</returns>
  Task<Result<DeleteAllDataModelsResponse>> DeleteAllDataModels(CancellationToken cancellationToken);

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

  /// <summary>
  /// Importuje všechny datové modely z AVAPlace (Source == AVAPlace) do lokální databáze,
  /// včetně jejich DataModelRecordů. Operace je idempotentní — lze ji bezpečně opakovaně spustit.
  /// Pokračuje i při chybě jednotlivého modelu (continue-on-error), chyby jsou vráceny v odpovědi.
  /// </summary>
  /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
  /// <returns>Souhrn výsledků importu jednotlivých modelů.</returns>
  Task<Result<ImportAllDataModelsFromAvaPlaceResponse>> ImportAllDataModelsFromAvaPlace(CancellationToken cancellationToken);

  /// <summary>
  /// Vrátí seznam oblastí ze zadaného datového zdroje.
  /// </summary>
  Task<Contracts.Areas.AreaListResponse> GetAreas(Datasource datasource, CancellationToken cancellationToken);

  /// <summary>
  /// Vrátí oblast podle identifikátoru.
  /// </summary>
  Task<AreaDTO> GetArea(Datasource datasource, Guid areaId, CancellationToken cancellationToken);

  /// <summary>
  /// Vrátí oblast podle kódu.
  /// </summary>
  Task<AreaDTO> GetArea(Datasource datasource, string areaCode, CancellationToken cancellationToken);

  /// <summary>
  /// Vytvoří novou oblast a vrátí výsledek obsahující ID nové oblasti.
  /// </summary>
  Task<Result<Guid>> CreateArea(Datasource datasource, AreaDTO area, CancellationToken cancellationToken);

  /// <summary>
  /// Aktualizuje oblast a vrátí výsledek obsahující aktualizovanou oblast.
  /// </summary>
  Task<Result<AreaDTO>> UpdateArea(Datasource datasource, AreaDTO area, CancellationToken cancellationToken);

  /// <summary>
  /// Smaže oblast podle kódu a vrátí výsledek operace.
  /// </summary>
  Task<Result> DeleteArea(Datasource datasource, string areaCode, CancellationToken cancellationToken);

  /// <summary>
  /// Vrátí seznam nasazení.
  /// </summary>
  Task<DeploymentListResponse> GetDeployments(CancellationToken cancellationToken);

  /// <summary>
  /// Vrátí seznam posledních <paramref name="count"/> nasazení.
  /// </summary>
  Task<DeploymentListResponse> GetRecentDeployments(int count = 5, CancellationToken cancellationToken = default);

  /// <summary>
  /// Vrátí nasazení podle identifikátoru.
  /// </summary>
  Task<DeploymentDTO> GetDeployment(Guid id, CancellationToken cancellationToken);

  /// <summary>
  /// Vrátí nasazení podle kódu.
  /// </summary>
  Task<DeploymentDTO> GetDeployment(string code, CancellationToken cancellationToken);

  /// <summary>
  /// Vytvoří nové nasazení.
  /// </summary>
  Task<Result<Guid>> CreateDeployment(DeploymentDTO deployment, CancellationToken cancellationToken);

  /// <summary>
  /// Aktualizuje nasazení.
  /// </summary>
  Task<Result<DeploymentDTO>> UpdateDeployment(DeploymentDTO deployment, CancellationToken cancellationToken);

  /// <summary>
  /// Smaže nasazení podle kódu.
  /// </summary>
  Task<Result> DeleteDeployment(string code, CancellationToken cancellationToken);

  /// <summary>
  /// Exportuje DataModely nasazení včetně jejich záznamů jako ZIP archív.
  /// </summary>
  Task<byte[]> ExportDeployment(string deploymentCode, CancellationToken cancellationToken);

  /// <summary>
  /// Vrátí hluboké porovnání datového modelu (včetně polí) mezi lokální databází a AVAPlace.
  /// </summary>
  /// <param name="dataModelId">Identifikátor datového modelu.</param>
  /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
  /// <returns>Výsledek porovnání nebo null pokud model nebyl nalezen.</returns>
  Task<DataModelComparisonDTO?> GetDataModelChanges(Guid dataModelId, CancellationToken cancellationToken);

  /// <summary>
  /// Vrátí souhrnné porovnání DataModelů nasazení mezi lokální databází a AVAPlace.
  /// DataModelIds jsou předány přímo — bez DB lookupu nasazení.
  /// </summary>
  /// <param name="deploymentCode">Kód nasazení (informativní).</param>
  /// <param name="deploymentName">Název nasazení (informativní).</param>
  /// <param name="dataModelIds">Identifikátory DataModelů nasazení.</param>
  /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
  Task<Contracts.DataModels.DeploymentChangesSummaryDTO?> GetDeploymentChangesSummary(
    string deploymentCode,
    string deploymentName,
    List<Guid> dataModelIds,
    CancellationToken cancellationToken);

  /// <summary>
  /// Vrátí souhrn statistik dashboardu (počty modelů a nasazení).
  /// </summary>
  Task<GetDashboardSummaryResponse> GetDashboardSummary(CancellationToken cancellationToken = default);
}
