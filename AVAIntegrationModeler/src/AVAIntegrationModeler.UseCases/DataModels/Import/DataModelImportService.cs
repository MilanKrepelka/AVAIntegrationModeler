using Ardalis.Specification;
using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.AreaAggregate;
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
/// Oblast (Area) se odvozuje ze statického mapování <see cref="DataModelAreaMapping"/> — AVAPlace tuto informaci neposkytuje.
/// </summary>
public class DataModelImportService(
  IDataModelRepository dataModelRepository,
  IDataModelRecordRepository recordRepository,
  IIntegrationDataProvider integrationDataProvider,
  IRepository<Area> areaRepository)
  : IDataModelImportService
{
  private Dictionary<string, Guid>? _areaCodeToId;
  public async Task<Result<Guid>> ImportModelAsync(DataModelDTO dto, CancellationToken ct = default)
  {
    try
    {
      var existing = await dataModelRepository.FirstOrDefaultAsync(
        new DataModelByCodeSpec(dto.Code), ct);

      var localModelId = existing is null
        ? await CreatePathAsync(dto, ct)
        : await UpdatePathAsync(existing, dto, ct);

      await ImportRecordsAsync(dto.Id, localModelId, ct);
      return Result<Guid>.Success(localModelId);
    }
    catch (Exception ex) when (ex is not OperationCanceledException)
    {
      return Result<Guid>.Error($"Import datového modelu '{dto.Code}' selhal: {ex.GetBaseException().Message}");
    }
  }

  private async Task<Guid> CreatePathAsync(DataModelDTO dto, CancellationToken ct)
  {
    DataModel dataModel;
    try
    {
      dataModel = new DataModel(dto.Id, dto.Code);
      dataModel
        .SetName(dto.Name)
        .SetDescription(dto.Description)
        .SetNotes(dto.Notes);
      if (dto.IsAggregateRoot) dataModel.MarkAsAggregateRoot();
      else                    dataModel.MarkAsNestedEntity();
      var resolvedAreaId = await ResolveAreaIdAsync(dto.Code, ct);
      if (resolvedAreaId.HasValue) dataModel.SetArea(resolvedAreaId.Value);
    }
    catch (ArgumentException ex)
    {
      throw new InvalidOperationException($"Neplatná vlastnost DataModelu: {ex.Message}", ex);
    }

    var created = await dataModelRepository.AddAsync(dataModel, ct);
    if (created is null) throw new InvalidOperationException("AddAsync vrátil null.");

    var (fieldsToAdd, fieldError) = BuildDataModelFields(created, dto.Fields);
    if (fieldError is not null)
      throw new InvalidOperationException($"Chyba při stavbě polí: {fieldError}");

    if (fieldsToAdd.Count > 0)
      await dataModelRepository.SyncFieldsAndSaveAsync(created, new List<DataModelField>(), fieldsToAdd, ct);

    return created.Id;
  }

  private async Task<Guid> UpdatePathAsync(DataModel existing, DataModelDTO dto, CancellationToken ct)
  {
    // Pokud se lokální Id liší od AVAPlace Id (záznam importovaný před opravou),
    // smažeme starý model a vytvoříme nový se správným Id.
    if (existing.Id != dto.Id)
    {
      await dataModelRepository.DeleteAsync(existing, ct);
      return await CreatePathAsync(dto, ct);
    }

    try
    {
      existing
        .SetCode(dto.Code)
        .SetName(dto.Name)
        .SetDescription(dto.Description)
        .SetNotes(dto.Notes);
      if (dto.IsAggregateRoot) existing.MarkAsAggregateRoot();
      else                    existing.MarkAsNestedEntity();
      var resolvedAreaId = await ResolveAreaIdAsync(dto.Code, ct);
      if (resolvedAreaId.HasValue) existing.SetArea(resolvedAreaId.Value);
    }
    catch (ArgumentException ex)
    {
      throw new InvalidOperationException($"Neplatná vlastnost DataModelu: {ex.Message}", ex);
    }

    var fieldsToDelete = existing.Fields.ToList();
    foreach (var name in fieldsToDelete.Select(f => f.Name))
      existing.RemoveField(name);

    var (fieldsToAdd, fieldError) = BuildDataModelFields(existing, dto.Fields);
    if (fieldError is not null)
      throw new InvalidOperationException($"Chyba při stavbě polí: {fieldError}");

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

      if (localRecord is null || localRecord.Id != avaRecord.Id)
      {
        if (localRecord is not null)
          await recordRepository.DeleteAsync(localRecord, ct);

        var newRecord = new DataModelRecord(avaRecord.Id, localModelId);
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
        var fieldId = fieldDto.Id != Guid.Empty ? fieldDto.Id : Guid.NewGuid();
        var field = new DataModelField(fieldId, fieldDto.Name, fieldDto.FieldType);
        if (!string.IsNullOrEmpty(fieldDto.Label)) field.SetLabel(fieldDto.Label);
        field.SetDescription(fieldDto.Description);
        if (fieldDto.IsPublishedForLookup) field.MarkAsPublishedForLookup();
        if (fieldDto.IsCollection)         field.MarkAsCollection();
        if (fieldDto.IsLocalized)          field.MarkAsLocalized();
        if (fieldDto.IsNullable)           field.MarkAsNullable();
        if (fieldDto.FieldType is DataModelFieldType.LookupEntity or DataModelFieldType.NestedEntity)
          foreach (var refId in fieldDto.ReferencedEntityTypeIds.Where(id => id != Guid.Empty))
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

  /// <summary>
  /// Načte slovník AreaCode → AreaId z databáze. Výsledek je cachován po dobu životnosti služby
  /// (scoped), takže hromadný import provede dotaz na oblasti pouze jednou.
  /// </summary>
  private async Task<Dictionary<string, Guid>> GetAreaLookupAsync(CancellationToken ct)
  {
    if (_areaCodeToId is null)
    {
      var areas = await areaRepository.ListAsync(ct);
      _areaCodeToId = areas.ToDictionary(a => a.Code, a => a.Id, StringComparer.OrdinalIgnoreCase);
    }
    return _areaCodeToId;
  }

  /// <summary>
  /// Vrátí Id oblasti pro daný název DataModelu ze statického mapování.
  /// Pokud oblast v lokální DB ještě neexistuje, vytvoří ji (Code = Name = kód z mapování).
  /// Vrací <see langword="null"/>, pokud název není v mapování.
  /// </summary>
  private async Task<Guid?> ResolveAreaIdAsync(string dataModelName, CancellationToken ct)
  {
    if (!DataModelAreaMapping.TryGetAreaCode(dataModelName, out var areaCode))
      return null;
    var lookup = await GetAreaLookupAsync(ct);
    if (lookup.TryGetValue(areaCode, out var existingId))
      return existingId;

    var newArea = new Area(Guid.NewGuid(), areaCode);
    newArea.SetName(areaCode);
    await areaRepository.AddAsync(newArea, ct);
    lookup[areaCode] = newArea.Id;
    return newArea.Id;
  }
}
