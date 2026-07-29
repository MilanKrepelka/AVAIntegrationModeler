using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Export;

namespace AVAIntegrationModeler.UseCases.DataModels.Export;

public class ExportDataModelsHandler(IDataModelQueryService _query)
  : IQueryHandler<ExportDataModelsQuery, Result<ExportResult<DataModelDTO>>>
{
  private static readonly char[] _invalidChars = Path.GetInvalidFileNameChars();

  public async Task<Result<ExportResult<DataModelDTO>>> Handle(ExportDataModelsQuery request, CancellationToken ct)
  {
    if (request.Datasource != Datasource.Database)
      return Result<ExportResult<DataModelDTO>>.Invalid(
        new ValidationError("Datasource", "Export je dostupný pouze pro Database."));

    var all = await _query.ListAsync(Datasource.Database);
    var entries = all
      .Where(m => request.ModelIds.Contains(m.Id))
      .Select(m => new ExportEntry<DataModelDTO>(SafeName(m.Code, m.Id) + ".json", m));

    return Result.Success(new ExportResult<DataModelDTO>("datamodels-export.zip", entries));
  }

  private static string SafeName(string? raw, Guid fallback)
  {
    var name = string.IsNullOrWhiteSpace(raw) ? fallback.ToString() : raw;
    return string.Concat(name.Select(c => _invalidChars.Contains(c) ? '_' : c));
  }
}
