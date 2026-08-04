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
}
