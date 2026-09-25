using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.DataModelRecords.Xls.ImportFromXls;

namespace AVAIntegrationModeler.API.DataModelRecords;

/// <summary>
/// Importuje záznamy datového modelu z XLSX souboru (multipart/form-data).
/// </summary>
public class ImportFromXls(IMediator _mediator) : Endpoint<ImportFromXlsRequest, ImportDataModelRecordsXlsResult>
{
  public override void Configure()
  {
    Post("/DataModelRecords/import-xls");
    AllowAnonymous();
    AllowFileUploads();
  }

  public override async Task HandleAsync(ImportFromXlsRequest req, CancellationToken ct)
  {
    if (req.ModelId == Guid.Empty || req.File is null)
    {
      await SendErrorsAsync(400, ct);
      return;
    }

    byte[] fileContent;
    using (var ms = new MemoryStream())
    {
      await req.File.CopyToAsync(ms, ct);
      fileContent = ms.ToArray();
    }

    var result = await _mediator.Send(
      new ImportDataModelRecordsFromXlsCommand(req.Datasource, req.ModelId, fileContent), ct);

    if (!result.IsSuccess)
    {
      await SendErrorsAsync(400, ct);
      return;
    }

    await SendOkAsync(result.Value, ct);
  }
}
