using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.DataModelRecordAggregate;

namespace AVAIntegrationModeler.UseCases.DataModelRecords.Create;

public class CreateDataModelRecordHandler(
  IRepository<DataModelRecord> repository,
  IDataModelRecordQueryService queryService)
  : ICommandHandler<CreateDataModelRecordCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateDataModelRecordCommand request, CancellationToken cancellationToken)
  {
    if (request.Datasource != Datasource.Database)
      return Result<Guid>.Invalid(
        new ValidationError("Datasource", "Záznamy datových modelů jsou dostupné pouze pro zdroj Database."));

    Guard.Against.Null(request.Record, nameof(request.Record));

    DataModelRecord record;
    try
    {
      record = DataModelRecordMapper.MapToEntity(request.Record);
    }
    catch (ArgumentException ex)
    {
      return Result<Guid>.Invalid(new ValidationError(ex.ParamName ?? "Record", ex.Message));
    }
    catch (InvalidOperationException ex)
    {
      return Result<Guid>.Error(ex.Message);
    }

    var created = await repository.AddAsync(record, cancellationToken);
    if (created is null) return Result<Guid>.Error("Nepodařilo se vytvořit záznam.");

    queryService.InvalidateCache(request.Datasource);
    return Result<Guid>.Success(created.Id);
  }
}
