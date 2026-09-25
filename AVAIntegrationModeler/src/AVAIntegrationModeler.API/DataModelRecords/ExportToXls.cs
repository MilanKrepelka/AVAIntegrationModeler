using AVAIntegrationModeler.UseCases.DataModelRecords.Xls.ExportToXls;

namespace AVAIntegrationModeler.API.DataModelRecords;

/// <summary>
/// Exportuje záznamy datového modelu do XLSX souboru.
/// </summary>
public class ExportToXls(IMediator _mediator) : Endpoint<ExportToXlsRequest>
{
  public override void Configure()
  {
    Get("/DataModelRecords/export-xls");
    AllowAnonymous();
  }

  public override async Task HandleAsync(ExportToXlsRequest req, CancellationToken ct)
  {
    if (req.ModelId == Guid.Empty)
    {
      await SendErrorsAsync(400, ct);
      return;
    }

    var result = await _mediator.Send(
      new ExportDataModelRecordsToXlsQuery(req.Datasource, req.ModelId), ct);

    if (result.IsNotFound())
    {
      await SendNotFoundAsync(ct);
      return;
    }

    if (!result.IsSuccess)
    {
      await SendErrorsAsync(400, ct);
      return;
    }

    var (content, fileName) = result.Value;
    var stream = new MemoryStream(content);
    await SendStreamAsync(stream, fileName: fileName,
      contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      cancellation: ct);
  }
}
