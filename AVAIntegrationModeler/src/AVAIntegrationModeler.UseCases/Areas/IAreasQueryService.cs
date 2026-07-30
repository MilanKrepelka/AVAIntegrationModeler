using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Areas;

/// <summary>
/// Dotazovací služba pro oblasti. Typicky implementována v Infrastructure.
/// </summary>
public interface IAreasQueryService : ICacheableQueryService
{
  /// <summary>
  /// Vrátí seznam všech oblastí pro zadaný datový zdroj.
  /// </summary>
  Task<IEnumerable<AreaDTO>> ListAsync(Contracts.Datasource datasource);

  /// <summary>
  /// Vrátí detail oblasti podle identifikátoru.
  /// </summary>
  Task<AreaDTO> GetArea(Contracts.Datasource datasource, Guid areaId, CancellationToken cancellationToken);

  /// <summary>
  /// Vrátí detail oblasti podle kódu.
  /// </summary>
  Task<AreaDTO> GetArea(Contracts.Datasource datasource, string areaCode, CancellationToken cancellationToken);

  /// <summary>
  /// Ověří existenci oblasti podle Id bez vyhazování výjimky.
  /// </summary>
  Task<bool> ExistsByIdAsync(Contracts.Datasource datasource, Guid areaId, CancellationToken cancellationToken);

  /// <summary>
  /// Ověří existenci oblasti podle kódu bez vyhazování výjimky.
  /// </summary>
  Task<bool> ExistsByCodeAsync(Contracts.Datasource datasource, string areaCode, CancellationToken cancellationToken);
}
