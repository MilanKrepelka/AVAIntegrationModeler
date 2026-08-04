using AVAIntegrationModeler.Domain.DeploymentAggregate;

namespace AVAIntegrationModeler.UseCases.Deployments;

/// <summary>
/// Rozšířené rozhraní repozitáře pro agregát <see cref="Deployment"/>.
/// Přidává operaci pro synchronizaci kolekce datových modelů bez kolize s EF Core change trackerem.
/// </summary>
public interface IDeploymentRepository : IRepository<Deployment>
{
  /// <summary>
  /// Synchronizuje datové modely nasazení a uloží změny v jedné transakci.
  /// Skalární vlastnosti nasazení jsou aktualizovány přes ExecuteUpdateAsync,
  /// odebrané záznamy přes ExecuteDeleteAsync — oba obejdou change-tracker cascade logiku.
  /// </summary>
  /// <param name="deployment">Nasazení s aktuálním stavem.</param>
  /// <param name="toDelete">Záznamy <see cref="DeploymentDataModel"/>, které mají být odstraněny.</param>
  /// <param name="toAdd">Záznamy <see cref="DeploymentDataModel"/>, které mají být přidány.</param>
  /// <param name="ct">Token zrušení.</param>
  Task SyncDataModelsAndSaveAsync(
    Deployment deployment,
    IList<DeploymentDataModel> toDelete,
    IList<DeploymentDataModel> toAdd,
    CancellationToken ct = default);
}
