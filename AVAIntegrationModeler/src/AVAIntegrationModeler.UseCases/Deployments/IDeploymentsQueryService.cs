using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.Deployments;

/// <summary>
/// Dotazovací služba pro nasazení. Typicky implementována v Infrastructure.
/// </summary>
public interface IDeploymentsQueryService : ICacheableQueryService
{
  /// <summary>
  /// Vrátí seznam všech nasazení.
  /// </summary>
  Task<IEnumerable<DeploymentDTO>> ListAsync();

  /// <summary>
  /// Vrátí detail nasazení podle identifikátoru.
  /// </summary>
  Task<DeploymentDTO> GetDeployment(Guid id, CancellationToken cancellationToken);

  /// <summary>
  /// Vrátí detail nasazení podle kódu.
  /// </summary>
  Task<DeploymentDTO> GetDeployment(string code, CancellationToken cancellationToken);

  /// <summary>
  /// Vrátí posledních <paramref name="count"/> nasazení.
  /// </summary>
  Task<IEnumerable<DeploymentDTO>> ListRecentAsync(int count, CancellationToken cancellationToken = default);

  /// <summary>
  /// Ověří existenci nasazení podle kódu bez vyhazování výjimky.
  /// </summary>
  Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken);
}
