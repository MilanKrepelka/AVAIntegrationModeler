using AVAIntegrationModeler.Domain.DataModelAggregate;
using AVAIntegrationModeler.UseCases.DataModels;
using Microsoft.EntityFrameworkCore;

namespace AVAIntegrationModeler.Infrastructure.Data;

public class DataModelRepository : EfRepository<DataModel>, IDataModelRepository
{
  private readonly AppDbContext _dbContext;

  public DataModelRepository(AppDbContext dbContext) : base(dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task SyncFieldsAndSaveAsync(
    DataModel model,
    IList<DataModelField> fieldsToDelete,
    IList<DataModelField> fieldsToAdd,
    CancellationToken ct = default)
  {
    await using var tx = await _dbContext.Database.BeginTransactionAsync(ct);
    try
    {
      // 1. Track new fields while DataModel is still tracked → relationship fixup sets DataModelId.
      if (fieldsToAdd.Count > 0)
        _dbContext.Set<DataModelField>().AddRange(fieldsToAdd);

      // 2. Delete old fields directly via SQL — bypasses change-tracker cascade logic.
      if (fieldsToDelete.Count > 0)
      {
        var fieldIds = fieldsToDelete.Select(f => f.Id).ToList();
        await _dbContext.Set<DataModelFieldEntityTypeReference>()
          .Where(r => fieldIds.Contains(r.DataModelFieldId))
          .ExecuteDeleteAsync(ct);
        await _dbContext.Set<DataModelField>()
          .Where(f => fieldIds.Contains(f.Id))
          .ExecuteDeleteAsync(ct);
      }

      // 3. Update DataModel scalars directly via SQL — bypasses DetectChanges / ValueGeneratedOnAdd issues.
      await _dbContext.Set<DataModel>()
        .Where(dm => dm.Id == model.Id)
        .ExecuteUpdateAsync(s => s
          .SetProperty(dm => dm.Code, model.Code)
          .SetProperty(dm => dm.Name, model.Name)
          .SetProperty(dm => dm.Description, model.Description)
          .SetProperty(dm => dm.Notes, model.Notes)
          .SetProperty(dm => dm.IsAggregateRoot, model.IsAggregateRoot)
          .SetProperty(dm => dm.AreaId, model.AreaId), ct);

      // 4. Detach old fields before resetting DataModel state — prevents orphan-detection
      //    from scheduling a second DELETE for rows that ExecuteDeleteAsync already removed.
      foreach (var field in fieldsToDelete)
      {
        foreach (var etRef in field.EntityTypeReferences)
          _dbContext.Entry(etRef).State = EntityState.Detached;
        _dbContext.Entry(field).State = EntityState.Detached;
      }

      // 5. Reset DataModel to Unchanged. This re-takes the navigation snapshot from the
      //    current _fields list (which now contains only new fields), so DetectChanges
      //    (a) won't find old fields in the snapshot and won't schedule Deletes for them,
      //    (b) won't detect scalar property changes and won't schedule a duplicate UPDATE.
      _dbContext.Entry(model).State = EntityState.Unchanged;

      // 6. Insert new fields and their EntityTypeRefs (DataModelId set by fixup in step 1).
      await _dbContext.SaveChangesAsync(ct);
      await tx.CommitAsync(ct);
    }
    catch
    {
      await tx.RollbackAsync(ct);
      throw;
    }
  }

  public async Task<int> DeleteAllAsync(CancellationToken ct = default)
  {
    await using var tx = await _dbContext.Database.BeginTransactionAsync(ct);
    try
    {
      await _dbContext.Set<DataModelFieldEntityTypeReference>().ExecuteDeleteAsync(ct);
      await _dbContext.Set<DataModelField>().ExecuteDeleteAsync(ct);
      var deletedCount = await _dbContext.Set<DataModel>().ExecuteDeleteAsync(ct);

      await tx.CommitAsync(ct);
      return deletedCount;
    }
    catch
    {
      await tx.RollbackAsync(ct);
      throw;
    }
  }
}
