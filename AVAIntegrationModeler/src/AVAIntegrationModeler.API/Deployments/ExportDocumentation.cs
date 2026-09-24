using System.IO.Compression;
using System.Text;
using AVAIntegrationModeler.UseCases.Deployments.ExportDocumentation;

namespace AVAIntegrationModeler.API.Deployments;

/// <summary>
/// Exportuje markdown dokumentaci nasazení jako ZIP archív.
/// Pro každý DataModel nasazení obsahuje soubor {area_code}/models/{model_code}-{model_id}.md
/// a soubor ListOfChanges.md s měsíčními rozdíly metadat.
/// </summary>
public class ExportDeploymentDocumentationEndpoint(IMediator _mediator)
  : Endpoint<ExportDeploymentDocumentationRequest>
{
  public override void Configure()
  {
    Get(ExportDeploymentDocumentationRequest.Route);
    AllowAnonymous();
    Summary(s => s.Summary = "Exportuje markdown dokumentaci nasazení jako ZIP.");
  }

  public override async Task HandleAsync(ExportDeploymentDocumentationRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(
      new ExportDeploymentDocumentationQuery(req.DeploymentCode, req.Months), ct);

    if (result.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

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
        await using var writer = new StreamWriter(entryStream, Encoding.UTF8);
        await writer.WriteAsync(entry.Data);
      }
    }

    zipStream.Position = 0;
    await SendStreamAsync(zipStream,
      fileName: result.Value.ZipFileName,
      contentType: "application/zip",
      cancellation: ct);
  }
}
