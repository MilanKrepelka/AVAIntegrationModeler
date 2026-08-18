using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.DataModelAggregate;
using AVAIntegrationModeler.Domain.DataModelAggregate.Specifications;
using AVAIntegrationModeler.Domain.DataModelRecordAggregate;
using AVAIntegrationModeler.Domain.DataModelRecordAggregate.Specifications;
using AVAIntegrationModeler.UseCases.DataModelRecords;

namespace AVAIntegrationModeler.UseCases.DataModels.Import;

/// <summary>
/// Implementace <see cref="IDataModelImportService"/> — upsert datového modelu podle Code
/// a jeho DataModelRecordů podle ExternalId. Sdílená mezi importem jednoho modelu
/// (<see cref="ImportDataModelHandler"/>) a hromadným importem (<see cref="ImportAllDataModelsHandler"/>).
/// </summary>
public class DataModelImportService(
  IDataModelRepository dataModelRepository,
  IDataModelRecordRepository recordRepository,
  IIntegrationDataProvider integrationDataProvider)
  : IDataModelImportService
{
  public async Task<Result<Guid>> ImportModelAsync(DataModelDTO dto, CancellationToken ct = default)
  {
    var existing = await dataModelRepository.FirstOrDefaultAsync(
      new DataModelByCodeSpec(dto.Code), ct);

    var localModelId = existing is null
      ? await CreatePathAsync(dto, ct)
      : await UpdatePathAsync(existing, dto, ct);

    if (localModelId == Guid.Empty)
      return Result<Guid>.Error($"Import datového modelu '{dto.Code}' selhal.");

    await ImportRecordsAsync(dto.Id, localModelId, ct);
    return Result<Guid>.Success(localModelId);
  }

  private async Task<Guid> CreatePathAsync(DataModelDTO dto, CancellationToken ct)
  {
    DataModel dataModel;
    try
    {
      dataModel = new DataModel(Guid.NewGuid(), dto.Code);
      dataModel
        .SetName(dto.Name)
        .SetDescription(dto.Description)
        .SetNotes(dto.Notes);
      if (dto.IsAggregateRoot) dataModel.MarkAsAggregateRoot();
      else                    dataModel.MarkAsNestedEntity();
      if (dto.AreaId.HasValue) dataModel.SetArea(dto.AreaId.Value);
    }
    catch (ArgumentException)
    {
      return Guid.Empty;
    }

    var created = await dataModelRepository.AddAsync(dataModel, ct);
    if (created is null) return Guid.Empty;

    var (fieldsToAdd, fieldError) = BuildDataModelFields(created, dto.Fields);
    if (fieldError is not null) return Guid.Empty;

    if (fieldsToAdd.Count > 0)
      await dataModelRepository.SyncFieldsAndSaveAsync(created, new List<DataModelField>(), fieldsToAdd, ct);

    return created.Id;
  }

  private async Task<Guid> UpdatePathAsync(DataModel existing, DataModelDTO dto, CancellationToken ct)
  {
    try
    {
      existing
        .SetCode(dto.Code)
        .SetName(dto.Name)
        .SetDescription(dto.Description)
        .SetNotes(dto.Notes);
      if (dto.IsAggregateRoot) existing.MarkAsAggregateRoot();
      else                    existing.MarkAsNestedEntity();
      if (dto.AreaId.HasValue) existing.SetArea(dto.AreaId.Value);
    }
    catch (ArgumentException)
    {
      return Guid.Empty;
    }

    var fieldsToDelete = existing.Fields.ToList();
    foreach (var name in fieldsToDelete.Select(f => f.Name))
      existing.RemoveField(name);

    var (fieldsToAdd, fieldError) = BuildDataModelFields(existing, dto.Fields);
    if (fieldError is not null) return Guid.Empty;

    await dataModelRepository.SyncFieldsAndSaveAsync(existing, fieldsToDelete, fieldsToAdd, ct);
    return existing.Id;
  }

  private async Task ImportRecordsAsync(Guid avaPlaceModelId, Guid localModelId, CancellationToken ct)
  {
    var avaRecords = (await integrationDataProvider.GetDataModelRecordsAsync(avaPlaceModelId, ct)).ToList();
    if (avaRecords.Count == 0) return;

    var existingRecords = await recordRepository.ListAsync(
      new DataModelRecordsByModelIdSpec(localModelId), ct);

    foreach (var avaRecord in avaRecords)
    {
      var localRecord = existingRecords
        .FirstOrDefault(r => !string.IsNullOrEmpty(avaRecord.ExternalId)
                          && r.ExternalId == avaRecord.ExternalId);

      if (localRecord is null)
      {
        var newRecord = new DataModelRecord(Guid.NewGuid(), localModelId);
        newRecord.SetExternalId(avaRecord.ExternalId);
        var created = await recordRepository.AddAsync(newRecord, ct);
        if (created is not null)
        {
          var fieldsToAdd = BuildRecordFields(created, avaRecord.Fields);
          if (fieldsToAdd.Count > 0)
            await recordRepository.SyncFieldsAndSaveAsync(created, new List<DataModelRecordField>(), fieldsToAdd, ct);
        }
      }
      else
      {
        var fieldsToDelete = localRecord.Fields.ToList();
        foreach (var key in fieldsToDelete.Select(f => f.Key))
          localRecord.RemoveField(key);
        var fieldsToAdd = BuildRecordFields(localRecord, avaRecord.Fields);
        await recordRepository.SyncFieldsAndSaveAsync(localRecord, fieldsToDelete, fieldsToAdd, ct);
      }
    }
  }

  private static (List<DataModelField> Fields, string? Error) BuildDataModelFields(
    DataModel model, List<DataModelFieldDTO> fieldDtos)
  {
    var fieldsToAdd = new List<DataModelField>();
    foreach (var fieldDto in fieldDtos)
    {
      try
      {
        var field = new DataModelField(Guid.NewGuid(), fieldDto.Name, fieldDto.FieldType);
        if (!string.IsNullOrEmpty(fieldDto.Label)) field.SetLabel(fieldDto.Label);
        field.SetDescription(fieldDto.Description);
        if (fieldDto.IsPublishedForLookup) field.MarkAsPublishedForLookup();
        if (fieldDto.IsCollection)         field.MarkAsCollection();
        if (fieldDto.IsLocalized)          field.MarkAsLocalized();
        if (fieldDto.IsNullable)           field.MarkAsNullable();
        if (fieldDto.FieldType is DataModelFieldType.LookupEntity or DataModelFieldType.NestedEntity)
          foreach (var refId in fieldDto.ReferencedEntityTypeIds)
            field.AddReferencedEntityType(refId);
        if (fieldDto.Expression is not null)
          field.SetExpression(fieldDto.Expression.Value, fieldDto.Expression.Order);
        model.AddField(field);
        fieldsToAdd.Add(field);
      }
      catch (ArgumentException ex)
      {
        return (new List<DataModelField>(), ex.Message);
      }
    }
    return (fieldsToAdd, null);
  }

  private static List<DataModelRecordField> BuildRecordFields(
    DataModelRecord record, List<DataModelRecordFieldDTO> fieldDtos)
  {
    var fieldsToAdd = new List<DataModelRecordField>();
    foreach (var fieldDto in fieldDtos)
    {
      var field = new DataModelRecordField(Guid.NewGuid(), fieldDto.Key);
      if (fieldDto.IsLocalized)
        field.SetLocalizedValue(fieldDto.CzechValue, fieldDto.EnglishValue);
      else
        field.SetStringValue(fieldDto.StringValue);
      record.AddField(field);
      fieldsToAdd.Add(field);
    }
    return fieldsToAdd;
  }
}
