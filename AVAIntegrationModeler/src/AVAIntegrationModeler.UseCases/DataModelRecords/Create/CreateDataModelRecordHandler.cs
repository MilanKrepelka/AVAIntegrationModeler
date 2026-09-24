using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain;
using AVAIntegrationModeler.Domain.DataModelRecordAggregate;
using AVAIntegrationModeler.Domain.DataModelRecordAggregate.Specifications;

namespace AVAIntegrationModeler.UseCases.DataModelRecords.Create;

public class CreateDataModelRecordHandler(
  IDataModelRecordRepository repository,
  IDataModelRecordQueryService queryService,
  IDomainEntityValidationService<DataModelRecord> recordValidationService)
  : ICommandHandler<CreateDataModelRecordCommand, Result<Guid>>
{
  public async Task<Result<Guid>> Handle(CreateDataModelRecordCommand request, CancellationToken cancellationToken)
  {
    if (request.Datasource != Datasource.Database)
      return Result<Guid>.Invalid(
        new ValidationError("Datasource", "Záznamy datových modelů jsou dostupné pouze pro zdroj Database."));

    Guard.Against.Null(request.Record, nameof(request.Record));

    var existing = await repository.FirstOrDefaultAsync(
      new DataModelRecordByIdSpec(request.Record.Id), cancellationToken);

    if (existing is not null)
      return await UpdateExisting(existing, request.Record, request.Datasource, cancellationToken);

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

    var validationResult = await recordValidationService.ValidateForCreate(request.Datasource, record, cancellationToken);
    if (!validationResult.IsSuccess)
      return validationResult;

    var created = await repository.AddAsync(record, cancellationToken);
    if (created is null) return Result<Guid>.Error("Nepodařilo se vytvořit záznam.");

    queryService.InvalidateCache(request.Datasource);
    return Result<Guid>.Success(created.Id);
  }

  private async Task<Result<Guid>> UpdateExisting(
    DataModelRecord existing, Contracts.DTO.DataModelRecordDTO dto, Datasource datasource, CancellationToken ct)
  {
    try { existing.SetExternalId(dto.ExternalId); }
    catch (ArgumentException ex) { return Result<Guid>.Invalid(new ValidationError(ex.ParamName ?? "Record", ex.Message)); }

    var validationResult = await recordValidationService.Validate(datasource, existing, ct);
    if (!validationResult.IsSuccess) return validationResult;

    var fieldsToDelete = existing.Fields.ToList();
    foreach (var key in fieldsToDelete.Select(f => f.Key))
      existing.RemoveField(key);

    foreach (var fieldDto in dto.Fields)
    {
      try
      {
        var field = new DataModelRecordField(Guid.NewGuid(), fieldDto.Key);
        if (fieldDto.IsLocalized) field.SetLocalizedValue(fieldDto.CzechValue, fieldDto.EnglishValue);
        else field.SetStringValue(fieldDto.StringValue);
        existing.AddField(field);
      }
      catch (ArgumentException ex)
      {
        return Result<Guid>.Invalid(new ValidationError(fieldDto.Key, ex.Message));
      }
    }

    await repository.SyncFieldsAndSaveAsync(existing, fieldsToDelete, existing.Fields.ToList(), ct);
    queryService.InvalidateCache(datasource);
    return Result<Guid>.Success(existing.Id);
  }
}
