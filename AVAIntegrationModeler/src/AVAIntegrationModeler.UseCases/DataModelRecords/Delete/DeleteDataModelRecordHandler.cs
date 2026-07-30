using AVAIntegrationModeler.Domain.DataModelRecordAggregate;
using AVAIntegrationModeler.Domain.DataModelRecordAggregate.Specifications;

namespace AVAIntegrationModeler.UseCases.DataModelRecords.Delete;

public class DeleteDataModelRecordHandler(
  IRepository<DataModelRecord> repository,
  IDataModelRecordQueryService queryService)
  : ICommandHandler<DeleteDataModelRecordCommand, Result>
{
  public async Task<Result> Handle(DeleteDataModelRecordCommand request, CancellationToken cancellationToken)
  {
    var record = await repository.FirstOrDefaultAsync(new DataModelRecordByIdSpec(request.RecordId), cancellationToken);
    if (record is null) return Result.NotFound();

    await repository.DeleteAsync(record, cancellationToken);
    queryService.InvalidateCache(request.Datasource);
    return Result.Success();
  }
}
