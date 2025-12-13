using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.API.Client;

/// <summary>
/// Klient pro AVA Integration Modeler API.
/// </summary>
public interface IAVAIntegrationModelerApiClient
{
  /// <summary>
  /// Vrátí všechny datové modely.
  /// </summary>
  /// <param name="datasource">Datový zdroj.</param>
  /// <param name="cancellationToken">Token pro zrušení operace.</param>
  /// <returns>Seznam datových modelů.</returns>
  public Task<DataModelListResponse> GetDataModels(Datasource datasource, CancellationToken cancellationToken);

}
