using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.DataModels;

/// <summary>
/// Rozhraní pro službu, která bude skutečně získávat data pro datové modely.
/// </summary>
public interface IDataModelQueryService
{
  /// <summary>
  /// Vrátí seznam shrnutí datových modelů pro daný zdroj dat.
  /// </summary>
  /// <param name="dataSource"><see cref="Contracts.Datasource"/></param>
  /// <returns>Seznam shrnutí datových modelů</returns>
  Task<IEnumerable<DataModelSummaryDTO>> ListSummaryAsync(Contracts.Datasource dataSource);

  /// <summary>
  /// Vrátí seznam datových modelů pro daný zdroj dat.
  /// </summary>
  /// <param name="dataSource"><see cref="Contracts.Datasource"/></param>
  /// <returns>Seznam datových modelů</returns>
  Task<IEnumerable<DataModelDTO>> ListAsync(Contracts.Datasource dataSource);
}
