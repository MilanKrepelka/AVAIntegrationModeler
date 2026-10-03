using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain;
using AVAIntegrationModeler.Domain.DataModelRecordAggregate;
using AVAIntegrationModeler.Domain.DataModelRecordAggregate.Specifications;
using AVAIntegrationModeler.UseCases.DataModelRecords.Xls;

namespace AVAIntegrationModeler.UseCases.DataModelRecords.Xls.ImportFromXls;

/// <summary>
/// Handler pro <see cref="ImportDataModelRecordsFromXlsCommand"/>.
/// Parsuje XLS soubor a pro každý řádek provede upsert záznamu datového modelu.
/// Záznamy s vyplněným Id jsou aktualizovány (pokud existují) nebo vytvořeny s daným Id.
/// Záznamy s prázdným Id jsou vždy vytvořeny jako nové.
/// Chyby na jednotlivých řádcích jsou shromažďovány a vráceny spolu s výsledkem.
/// </summary>
public class ImportDataModelRecordsFromXlsHandler(
  IDataModelRecordRepository _repository,
  IDataModelRecordQueryService _queryService,
  IDomainEntityValidationService<DataModelRecord> _validationService)
  : ICommandHandler<ImportDataModelRecordsFromXlsCommand, Result<ImportDataModelRecordsXlsResult>>
{
  public async Task<Result<ImportDataModelRecordsXlsResult>> Handle(
    ImportDataModelRecordsFromXlsCommand request, CancellationToken ct)
  {
    if (request.Datasource != Datasource.Database)
      return Result<ImportDataModelRecordsXlsResult>.Invalid(
        new ValidationError("Datasource", "Import z XLS je dostupný pouze pro Database."));

    var (parsed, parseErrors) = DataModelRecordXlsService.ParseXls(request.FileContent, request.ModelId);

    if (parseErrors.Count > 0)
      return Result.Success(new ImportDataModelRecordsXlsResult(0, 0, parseErrors));

    var rowErrors = new List<XlsImportRowError>();
    var created = 0;
    var updated = 0;
    var rowNumber = 2;

    foreach (var row in parsed)
    {
      try
      {
        DataModelRecord? existing = null;
        if (row.Id != Guid.Empty)
          existing = await _repository.FirstOrDefaultAsync(
            new DataModelRecordByIdSpec(row.Id), ct);

        if (existing is not null)
        {
          var updateError = await UpdateExistingAsync(existing, row, request.Datasource, ct);
          if (updateError is not null)
            rowErrors.Add(new XlsImportRowError(rowNumber, updateError));
          else
            updated++;
        }
        else
        {
          var createError = await CreateNewAsync(row, request.Datasource, ct);
          if (createError is not null)
            rowErrors.Add(new XlsImportRowError(rowNumber, createError));
          else
            created++;
        }
      }
      catch (Exception ex)
      {
        rowErrors.Add(new XlsImportRowError(rowNumber, ex.Message));
      }

      rowNumber++;
    }

    if (created > 0 || updated > 0)
      _queryService.InvalidateCache(request.Datasource);

    return Result.Success(new ImportDataModelRecordsXlsResult(created, updated, rowErrors));
  }

  private async Task<string?> UpdateExistingAsync(
    DataModelRecord existing, ParsedXlsRecord row, Datasource datasource, CancellationToken ct)
  {
    try { existing.SetExternalId(row.ExternalId); }
    catch (ArgumentException ex) { return ex.Message; }

    var validation = await _validationService.Validate(datasource, existing, ct);
    if (!validation.IsSuccess)
      return string.Join("; ", validation.ValidationErrors.Select(e => e.ErrorMessage));

    var toDelete = existing.Fields.ToList();
    foreach (var key in toDelete.Select(f => f.Key))
      existing.RemoveField(key);

    var fieldError = ApplyFields(existing, row.Fields);
    if (fieldError is not null) return fieldError;

    await _repository.SyncFieldsAndSaveAsync(existing, toDelete, existing.Fields.ToList(), ct);
    return null;
  }

  private async Task<string?> CreateNewAsync(ParsedXlsRecord row, Datasource datasource, CancellationToken ct)
  {
    var record = new DataModelRecord(
      row.Id == Guid.Empty ? Guid.NewGuid() : row.Id,
      row.ModelId);

    try { record.SetExternalId(row.ExternalId); }
    catch (ArgumentException ex) { return ex.Message; }

    var fieldError = ApplyFields(record, row.Fields);
    if (fieldError is not null) return fieldError;

    var validation = await _validationService.ValidateForCreate(datasource, record, ct);
    if (!validation.IsSuccess)
      return string.Join("; ", validation.ValidationErrors.Select(e => e.ErrorMessage));

    var created = await _repository.AddAsync(record, ct);
    if (created is null) return "Nepodařilo se vytvořit záznam.";

    return null;
  }

  private static string? ApplyFields(DataModelRecord record, List<DataModelRecordFieldDTO> fields)
  {
    foreach (var fieldDto in fields)
    {
      try
      {
        var field = new DataModelRecordField(Guid.NewGuid(), fieldDto.Key);
        if (fieldDto.IsLocalized)
          field.SetLocalizedValue(fieldDto.CzechValue, fieldDto.EnglishValue);
        else
          field.SetStringValue(fieldDto.StringValue);
        record.AddField(field);
      }
      catch (Exception ex)
      {
        return $"Pole '{fieldDto.Key}': {ex.Message}";
      }
    }

    return null;
  }
}
