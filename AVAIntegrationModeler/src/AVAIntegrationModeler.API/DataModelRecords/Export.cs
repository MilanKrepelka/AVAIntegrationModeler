using System.IO.Compression;
using System.Text.Json;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.UseCases.DataModelRecords;

namespace AVAIntegrationModeler.API.DataModelRecords;

public class Export(IDataModelRecordQueryService _queryService) : Endpoint<ExportDataModelRecordsRequest>
{
  private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

  private static readonly char[] _invalidChars = Path.GetInvalidFileNameChars();

  public override void Configure()
  {
    Post("/DataModelRecords/export");
    AllowAnonymous();
  }

  public override async Task HandleAsync(ExportDataModelRecordsRequest req, CancellationToken ct)
  {
    if (req.Datasource != Datasource.Database)
    {
      await SendErrorsAsync(400, ct);
      return;
    }

    if (req.RecordIds is null || req.RecordIds.Count == 0)
    {
      await SendErrorsAsync(400, ct);
      return;
    }

    var zipStream = new MemoryStream();

    using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
    {
      foreach (var id in req.RecordIds)
      {
        var dto = await _queryService.GetByIdAsync(Datasource.Database, id, ct);
        if (dto is null) continue;

        var rawName = string.IsNullOrWhiteSpace(dto.ExternalId) ? dto.Id.ToString() : dto.ExternalId;
        var safeName = string.Concat(rawName.Select(c => _invalidChars.Contains(c) ? '_' : c));
        var entry = archive.CreateEntry($"{safeName}.json", CompressionLevel.Optimal);

        await using var entryStream = entry.Open();
        await JsonSerializer.SerializeAsync(entryStream, dto, _jsonOptions, ct);
      }
    }

    zipStream.Position = 0;
    await SendStreamAsync(zipStream, fileName: "export.zip", contentType: "application/zip", cancellation: ct);
  }
}
