using System.IO.Compression;
using System.Text.Json;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.UseCases.DataModels;

namespace AVAIntegrationModeler.API.DataModels;

public class Export(IDataModelQueryService _queryService) : Endpoint<ExportDataModelsRequest>
{
  private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

  private static readonly char[] _invalidChars = Path.GetInvalidFileNameChars();

  public override void Configure()
  {
    Post("/DataModels/export");
    AllowAnonymous();
  }

  public override async Task HandleAsync(ExportDataModelsRequest req, CancellationToken ct)
  {
    if (req.Datasource != Datasource.Database)
    {
      await SendErrorsAsync(400, ct);
      return;
    }

    if (req.ModelIds is null || req.ModelIds.Count == 0)
    {
      await SendErrorsAsync(400, ct);
      return;
    }

    var all = await _queryService.ListAsync(Datasource.Database);
    var selected = all.Where(m => req.ModelIds.Contains(m.Id)).ToList();

    var zipStream = new MemoryStream();

    using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
    {
      foreach (var dto in selected)
      {
        var rawName = string.IsNullOrWhiteSpace(dto.Code) ? dto.Id.ToString() : dto.Code;
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
