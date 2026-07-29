using System.IO.Compression;
using System.Text.Json;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.UseCases.DataModels.Export;

namespace AVAIntegrationModeler.API.DataModels;

public class Export(IMediator _mediator) : Endpoint<ExportDataModelsRequest>
{
  private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

  public override void Configure()
  {
    Post("/DataModels/export");
    AllowAnonymous();
  }

  public override async Task HandleAsync(ExportDataModelsRequest req, CancellationToken ct)
  {
    if (req.ModelIds is null || req.ModelIds.Count == 0)
    {
      await SendErrorsAsync(400, ct);
      return;
    }

    var result = await _mediator.Send(new ExportDataModelsQuery(req.Datasource, req.ModelIds), ct);

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
        await JsonSerializer.SerializeAsync(entryStream, entry.Data, _jsonOptions, ct);
      }
    }

    zipStream.Position = 0;
    await SendStreamAsync(zipStream, fileName: result.Value.ZipFileName, contentType: "application/zip", cancellation: ct);
  }
}
