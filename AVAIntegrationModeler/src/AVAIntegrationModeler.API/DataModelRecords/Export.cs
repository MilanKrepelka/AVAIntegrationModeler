using System.IO.Compression;
using System.Text.Json;
using AVAIntegrationModeler.API.Serialization;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.DataModelRecords.Export;

namespace AVAIntegrationModeler.API.DataModelRecords;

public class Export(IMediator _mediator) : Endpoint<ExportDataModelRecordsRequest>
{

  public override void Configure()
  {
    Post("/DataModelRecords/export");
    AllowAnonymous();
  }

  public override async Task HandleAsync(ExportDataModelRecordsRequest req, CancellationToken ct)
  {
    if (req.RecordIds is null || req.RecordIds.Count == 0)
    {
      await SendErrorsAsync(400, ct);
      return;
    }

    var result = await _mediator.Send(new ExportDataModelRecordsQuery(req.Datasource, req.RecordIds), ct);

    if (!result.IsSuccess)
    {
      await SendErrorsAsync(400, ct);
      return;
    }

    var zipStream = new MemoryStream();

    using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
    {
      foreach (var entry in result.Value.Entries)
      {
        var zipEntry = archive.CreateEntry(entry.FileName, CompressionLevel.Optimal);
        await using var entryStream = zipEntry.Open();
        await JsonSerializer.SerializeAsync(entryStream, DataModelRecordExportMapper.MapToExport(entry.Data), ExportJsonOptions.Instance, ct);
      }
    }

    zipStream.Position = 0;
    await SendStreamAsync(zipStream, fileName: result.Value.ZipFileName, contentType: "application/zip", cancellation: ct);
  }
}
