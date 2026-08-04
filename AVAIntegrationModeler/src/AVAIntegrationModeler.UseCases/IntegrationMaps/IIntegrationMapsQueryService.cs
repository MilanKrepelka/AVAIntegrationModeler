using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.IntegrationMaps;

/// <summary>
/// Represents a service that will actually fetch the necessary data
/// Typically implemented in Infrastructure
/// </summary>
public interface IIntegrationMapsQueryService
{
  /// <summary>
  /// Vrátí seznam všech Integration Map jako shrnutí (bez detailních informací)
  /// </summary>
  /// <param name="dataSource"><see cref="Contracts.Datasource"/></param>
  /// <returns>Seznam shrnutí Integration Map</returns>
  Task<IEnumerable<IntegrationMapSummaryDTO>> ListSummaryAsync(Contracts.Datasource dataSource);

  /// <summary>
  /// Vrátí seznam všech Integration Map s detailními informacemi
  /// </summary>
  /// <param name="dataSource"><see cref="Contracts.Datasource"/></param>
  /// <returns>Seznam detailních informací o Integration Map</returns>
  Task<IEnumerable<IntegrationMapDTO>> ListAsync(Contracts.Datasource dataSource);
}
