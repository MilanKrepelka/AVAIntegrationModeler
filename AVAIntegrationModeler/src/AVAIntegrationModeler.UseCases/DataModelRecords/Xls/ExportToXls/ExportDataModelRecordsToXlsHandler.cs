using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.UseCases.DataModels;

namespace AVAIntegrationModeler.UseCases.DataModelRecords.Xls.ExportToXls;

/// <summary>
/// Handler pro <see cref="ExportDataModelRecordsToXlsQuery"/>.
/// Exportuje záznamy zvoleného datového modelu do XLSX souboru.
/// Název souboru je ve formátu <c>records-{ModelCode}-{yyyy-MM-dd}.xlsx</c>.
/// </summary>
public class ExportDataModelRecordsToXlsHandler(
  IDataModelRecordQueryService _records,
  IDataModelQueryService _models)
  : IQueryHandler<ExportDataModelRecordsToXlsQuery, Result<(byte[] Content, string FileName)>>
{
  public async Task<Result<(byte[] Content, string FileName)>> Handle(
    ExportDataModelRecordsToXlsQuery request, CancellationToken ct)
  {
    if (request.Datasource != Datasource.Database)
      return Result<(byte[], string)>.Invalid(
        new ValidationError("Datasource", "Export do XLS je dostupný pouze pro Database."));

    var model = (await _models.ListAsync(request.Datasource))
      .FirstOrDefault(m => m.Id == request.ModelId);

    if (model is null)
      return Result<(byte[], string)>.NotFound($"Datový model s Id {request.ModelId} nebyl nalezen.");

    var records = await _records.ListAsync(
      request.Datasource, request.ModelId, cancellationToken: ct);

    var content = DataModelRecordXlsService.BuildXls(records);
    var date = DateTime.UtcNow.ToString("yyyy-MM-dd");
    var fileName = $"records-{Sanitize(model.Code)}-{date}.xlsx";

    return Result.Success((content, fileName));
  }

  private static string SafeChars { get; } = new string(
    Path.GetInvalidFileNameChars()
      .Concat(Path.GetInvalidPathChars())
      .Distinct()
      .ToArray());

  private static string Sanitize(string name) =>
    string.IsNullOrWhiteSpace(name)
      ? "model"
      : string.Concat(name.Select(c => SafeChars.Contains(c) ? '_' : c));
}
