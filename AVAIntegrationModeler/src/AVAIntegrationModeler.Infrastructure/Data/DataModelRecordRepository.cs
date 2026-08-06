using AVAIntegrationModeler.Domain.DataModelRecordAggregate;
using AVAIntegrationModeler.UseCases.DataModelRecords;
using Microsoft.EntityFrameworkCore;

namespace AVAIntegrationModeler.Infrastructure.Data;

public class DataModelRecordRepository : EfRepository<DataModelRecord>, IDataModelRecordRepository
{
  private readonly AppDbContext _dbContext;

  public DataModelRecordRepository(AppDbContext dbContext) : base(dbContext)
  {
    _dbContext = dbContext;
  }

  public async Task SyncFieldsAndSaveAsync(
    DataModelRecord record,
    IList<DataModelRecordField> fieldsToDelete,
    IList<DataModelRecordField> fieldsToAdd,
    CancellationToken ct = default)
  {
    if (fieldsToDelete.Count > 0)
      _dbContext.Set<DataModelRecordField>().RemoveRange(fieldsToDelete);

    if (fieldsToAdd.Count > 0)
      _dbContext.Set<DataModelRecordField>().AddRange(fieldsToAdd);

    _dbContext.Entry(record).State = EntityState.Modified;

    await _dbContext.SaveChangesAsync(ct);
  }

  public async Task<int> DeleteAllAsync(CancellationToken ct = default)
  {
    await using var tx = await _dbContext.Database.BeginTransactionAsync(ct);
    try
    {
      await _dbContext.Set<DataModelRecordField>().ExecuteDeleteAsync(ct);
      var deletedCount = await _dbContext.Set<DataModelRecord>().ExecuteDeleteAsync(ct);

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
