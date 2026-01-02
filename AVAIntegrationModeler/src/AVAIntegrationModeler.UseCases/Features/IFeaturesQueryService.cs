using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Features;

/// <summary>
/// Represents a service that will actually fetch the necessary data
/// Typically implemented in Infrastructure
/// </summary>
public interface IFeaturesQueryService
{
  Task<IEnumerable<FeatureSummaryDTO>> ListSummaryAsync(Contracts.Datasource dataSource);

  Task<IEnumerable<FeatureDTO>> ListAsync(Contracts.Datasource dataSource);

  /// <summary>
  /// Vrátí detail feature
  /// </summary>
  /// <param name="dataSource"><see cref="Contracts.Datasource"/></param>
  /// <param name="featureId">Identifikátor feature</param>
  /// <param name="cancellationToken"><see cref="CancellationToken"></param>
  /// <returns>Integrační feature</returns>
  Task<FeatureDTO> GetFeature(Contracts.Datasource dataSource, Guid featureId, CancellationToken cancellationToken);

  /// <summary>
  /// Vrátí detail feature
  /// </summary>
  /// <param name="dataSource"><see cref="Contracts.Datasource"/></param>
  /// <param name="featureId">Identifikátor feature</param>
  /// <param name="cancellationToken"><see cref="CancellationToken"></param>
  /// <returns>Integrační feature</returns>
  Task<FeatureSummaryDTO> GetFeatureSummary(Contracts.Datasource dataSource, Guid featureId, CancellationToken cancellationToken);


  /// <summary>
  /// Vrátí detail feature
  /// </summary>
  /// <param name="dataSource"><see cref="Contracts.Datasource"/></param>
  /// <param name="featureCode">Kód feature</param>
  /// <param name="cancellationToken"><see cref="CancellationToken"></param>  
  /// <returns>Integrační feature</returns>
  Task<FeatureDTO> GetFeature(Contracts.Datasource dataSource, string featureCode, CancellationToken cancellationToken);

  /// <summary>
  /// Vrátí detail feature
  /// </summary>
  /// <param name="dataSource"><see cref="Contracts.Datasource"/></param>
  /// <param name="featureCode">Kód feature</param>
  /// <param name="cancellationToken"><see cref="CancellationToken"></param>  
  /// <returns>Integrační feature</returns>
  Task<FeatureSummaryDTO> GetFeatureSummary(Contracts.Datasource dataSource, string featureCode, CancellationToken cancellationToken);


  /// <summary>
  /// Ověří existenci feature podle Id bez vyhazování výjimky.
  /// </summary>
  Task<bool> ExistsByIdAsync(Contracts.Datasource dataSource, Guid featureId, CancellationToken cancellationToken);

  /// <summary>
  /// Ověří existenci feature podle kódu bez vyhazování výjimky.
  /// </summary>
  Task<bool> ExistsByCodeAsync(Contracts.Datasource dataSource, string featureCode, CancellationToken cancellationToken);
}
