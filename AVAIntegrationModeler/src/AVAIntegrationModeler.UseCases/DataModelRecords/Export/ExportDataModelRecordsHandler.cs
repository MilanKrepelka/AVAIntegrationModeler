using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Export;

namespace AVAIntegrationModeler.UseCases.DataModelRecords.Export;

public class ExportDataModelRecordsHandler(IDataModelRecordQueryService _query)
  : IQueryHandler<ExportDataModelRecordsQuery, Result<ExportResult<DataModelRecordDTO>>>
{
  private static readonly char[] _invalidChars = Path.GetInvalidFileNameChars();

  public async Task<Result<ExportResult<DataModelRecordDTO>>> Handle(ExportDataModelRecordsQuery request, CancellationToken ct)
  {
    if (request.Datasource != Datasource.Database)
      return Result<ExportResult<DataModelRecordDTO>>.Invalid(
        new ValidationError("Datasource", "Export je dostupný pouze pro Database."));

    var entries = new List<ExportEntry<DataModelRecordDTO>>();
    foreach (var id in request.RecordIds)
    {
      var dto = await _query.GetByIdAsync(Datasource.Database, id, ct);
      if (dto is not null)
        entries.Add(new ExportEntry<DataModelRecordDTO>(SafeName(dto.ExternalId, dto.Id) + ".json", dto));
    }

    return Result.Success(new ExportResult<DataModelRecordDTO>("records-export.zip", entries));
  }

  private static string SafeName(string? raw, Guid fallback)
  {
    var name = string.IsNullOrWhiteSpace(raw) ? fallback.ToString() : raw;
    return string.Concat(name.Select(c => _invalidChars.Contains(c) ? '_' : c));
  }
}
