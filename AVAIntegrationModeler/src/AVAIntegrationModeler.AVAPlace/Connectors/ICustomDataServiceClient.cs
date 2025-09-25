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
  }
