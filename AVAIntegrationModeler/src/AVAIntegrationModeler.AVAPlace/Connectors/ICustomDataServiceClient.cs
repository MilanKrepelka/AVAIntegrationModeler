using System.Threading;
using System.Threading.Tasks;
using ASOL.DataService.Connector;
using ASOL.DataService.Contracts;

namespace AVAIntegrationModeler.AVAPlace;

/// <summary>
/// Represents the custom client of DataService client.
/// </summary>
/// <summary>
/// Represents the custom client of PlatformStore Order API service extending its standard connector.
/// </summary>
public interface ICustomDataServiceClient : IDataServiceClient
{
  /// <summary>
  /// Get Data Agent By Code
  /// </summary>
  /// <param name="agentCode"></param>
  /// <param name="acceptNotFound"></param>
  /// <param name="ct"></param>
  /// <returns></returns>
  Task<DataAgentDefinition?> GetDataAgentByCodeAsync(string agentCode, bool acceptNotFound = false, CancellationToken ct = default);

  /// <summary>
  /// Switch Enabled Data Agent
  /// </summary>
  /// <param name="dataAgentId"></param>
  /// <param name="enabled"></param>
  /// <param name="ct"></param>
  /// <returns></returns>
  Task<bool> SwitchEnabledDataAgentAsync(string dataAgentId, bool enabled, CancellationToken ct = default);

  /// <summary>
  /// Switch Enabled Data Source
  /// </summary>
  /// <param name="dataSourceId"></param>
  /// <param name="enabled"></param>
  /// <param name="ct"></param>
  /// <returns></returns>
  Task<bool> SwitchEnabledDataSourceAsync(string dataSourceId, bool enabled, CancellationToken ct = default);

  /// <summary>
  /// Vrátí unifikovaná data pro daný model.
  /// </summary>
  /// <param name="ModelId">Identifikátor modelu</param>
  /// <param name="ct">Token pro zrušení operace</param>
  /// <returns>Seznam unifikovaných datových objektů</returns>
  Task<IList<Models.DataModelRecord>> GetUnifiedDataAsync(Guid ModelId, CancellationToken ct = default);

  /// <summary>
  /// Vytvoří novou verzi metadat v DataService a vrátí její kód.
  /// </summary>
  /// <param name="ct">Token pro zrušení operace</param>
  /// <returns>Kód nově vytvořené verze metadat</returns>
  Task<string> CreateMetadataVersionAsync(CancellationToken ct = default);

  /// <summary>
  /// Importuje definici datového modelu (JSON) do DataService pro zadanou verzi metadat.
  /// Odpovídá funkci _Import-DataFromFile z PowerShell skriptu — POST na /api/v1/process/importdatamodel.
  /// </summary>
  /// <param name="targetVersion">Kód cílové verze metadat (získaný z <see cref="CreateMetadataVersionAsync"/>)</param>
  /// <param name="allowUpdate">Povolí přepsání existující definice modelu</param>
  /// <param name="jsonContent">Stream s obsahem JSON souboru definice modelu</param>
  /// <param name="ct">Token pro zrušení operace</param>
  Task ImportDataModelAsync(string targetVersion, bool allowUpdate, Stream jsonContent, CancellationToken ct = default);
}
