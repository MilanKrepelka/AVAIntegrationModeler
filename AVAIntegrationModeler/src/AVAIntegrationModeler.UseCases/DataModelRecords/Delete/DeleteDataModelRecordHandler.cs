using AVAIntegrationModeler.Contracts;
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
    if (request.Datasource != Datasource.Database)
      return Result.Invalid(new ValidationError("Datasource", "Záznamy datových modelů jsou dostupné pouze pro zdroj Database."));

    var record = await repository.FirstOrDefaultAsync(new DataModelRecordByIdSpec(request.RecordId), cancellationToken);
    if (record is null) return Result.NotFound();

    await repository.DeleteAsync(record, cancellationToken);
    queryService.InvalidateCache(request.Datasource);
    return Result.Success();
  }
}
