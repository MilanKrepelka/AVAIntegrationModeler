using AVAIntegrationModeler.Domain.DeploymentAggregate;
using AVAIntegrationModeler.UseCases.Deployments;
using Microsoft.EntityFrameworkCore;

namespace AVAIntegrationModeler.Infrastructure.Data;

/// <summary>
/// Repozitář pro agregát <see cref="Deployment"/> s podporou synchronizace datových modelů.
/// </summary>
public class DeploymentRepository : EfRepository<Deployment>, IDeploymentRepository
{
  private readonly AppDbContext _dbContext;

  /// <summary>
  /// Inicializuje repozitář.
  /// </summary>
  public DeploymentRepository(AppDbContext dbContext) : base(dbContext)
  {
    _dbContext = dbContext;
  }

  /// <inheritdoc />
  public async Task SyncDataModelsAndSaveAsync(
    Deployment deployment,
    IList<DeploymentDataModel> toDelete,
    IList<DeploymentDataModel> toAdd,
    CancellationToken ct = default)
  {
    await using var tx = await _dbContext.Database.BeginTransactionAsync(ct);
    try
    {
      // 1. Přidat nové záznamy do change trackeru — vztah nastavuje DeploymentId přes fixup.
      if (toAdd.Count > 0)
        _dbContext.Set<DeploymentDataModel>().AddRange(toAdd);

      // 2. Smazat staré záznamy přímo přes SQL — obejde change-tracker cascade logiku.
      if (toDelete.Count > 0)
      {
        var ids = toDelete.Select(m => m.Id).ToList();
        await _dbContext.Set<DeploymentDataModel>()
          .Where(m => ids.Contains(m.Id))
          .ExecuteDeleteAsync(ct);
      }

      // 3. Aktualizovat skalární vlastnosti přes SQL — obejde DetectChanges/ValueGeneratedOnAdd.
      await _dbContext.Set<Deployment>()
        .Where(d => d.Id == deployment.Id)
        .ExecuteUpdateAsync(s => s
          .SetProperty(d => d.Code, deployment.Code)
          .SetProperty(d => d.Name, deployment.Name)
          .SetProperty(d => d.Ticket, deployment.Ticket)
          .SetProperty(d => d.Description, deployment.Description)
          .SetProperty(d => d.LastSaveDateTime, deployment.LastSaveDateTime)
          .SetProperty(d => d.LastDeploymentDateTime, deployment.LastDeploymentDateTime), ct);

      // 4. Odpojit smazané záznamy, aby je DetectChanges nenaplánoval znovu na DELETE.
      foreach (var item in toDelete)
        _dbContext.Entry(item).State = EntityState.Detached;

      // 5. Resetovat stav Deployment na Unchanged — snapshot navigace se překreslí
      //    ze stavu _dataModels (bez starých, s novými položkami),
      //    čímž DetectChanges nenajde skalární změny k duplikovatelné UPDATE.
      _dbContext.Entry(deployment).State = EntityState.Unchanged;

      // 6. Uložit nové DeploymentDataModel záznamy (DeploymentId byl nastaven fixupem v kroku 1).
      await _dbContext.SaveChangesAsync(ct);
      await tx.CommitAsync(ct);
    }
    catch
    {
      await tx.RollbackAsync(ct);
      throw;
    }
  }
}
