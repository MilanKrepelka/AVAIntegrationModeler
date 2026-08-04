using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.DataModelRecordAggregate;
using AVAIntegrationModeler.Domain.DataModelRecordAggregate.Specifications;

namespace AVAIntegrationModeler.UseCases.DataModelRecords.Update;

public class UpdateDataModelRecordHandler(
  IDataModelRecordRepository repository,
  IDataModelRecordQueryService queryService)
  : ICommandHandler<UpdateDataModelRecordCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(UpdateDataModelRecordCommand request, CancellationToken cancellationToken)
  {
    if (request.Datasource != Datasource.Database)
      return Result<Guid>.Invalid(
        new ValidationError("Datasource", "Záznamy datových modelů jsou dostupné pouze pro zdroj Database."));

    Guard.Against.Null(request.Record, nameof(request.Record));

    var existing = await repository.FirstOrDefaultAsync(
      new DataModelRecordByIdSpec(request.Record.Id), cancellationToken);
    if (existing is null) return Result<Guid>.NotFound();

    try
    {
      existing.SetExternalId(request.Record.ExternalId);
    }
    catch (ArgumentException ex)
    {
      return Result<Guid>.Invalid(new ValidationError(ex.ParamName ?? "Record", ex.Message));
    }

    var fieldsToDelete = existing.Fields.ToList();
    foreach (var key in fieldsToDelete.Select(f => f.Key))
      existing.RemoveField(key);

    var fieldsToAdd = new List<DataModelRecordField>();
    foreach (var fieldDto in request.Record.Fields)
    {
      try
      {
        var field = new DataModelRecordField(Guid.NewGuid(), fieldDto.Key);
        if (fieldDto.IsLocalized)
          field.SetLocalizedValue(fieldDto.CzechValue, fieldDto.EnglishValue);
        else
          field.SetStringValue(fieldDto.StringValue);
        existing.AddField(field);
        fieldsToAdd.Add(field);
      }
      catch (ArgumentException ex)
      {
        return Result<Guid>.Invalid(new ValidationError(fieldDto.Key, ex.Message));
      }
    }

    await repository.SyncFieldsAndSaveAsync(existing, fieldsToDelete, fieldsToAdd, cancellationToken);
    queryService.InvalidateCache(request.Datasource);
    return Result<Guid>.Success(existing.Id);
  }
}
