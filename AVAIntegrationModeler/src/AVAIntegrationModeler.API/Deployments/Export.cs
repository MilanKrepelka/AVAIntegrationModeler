using System.IO.Compression;
using System.Text.Json;
using AVAIntegrationModeler.API.Serialization;
using AVAIntegrationModeler.AVAPlace.Mapping;
using AVAIntegrationModeler.UseCases.Deployments.Export;

namespace AVAIntegrationModeler.API.Deployments;

/// <summary>
/// Exportuje DataModely nasazení včetně jejich záznamů jako ZIP archív.
/// </summary>
public class ExportDeploymentEndpoint(IMediator _mediator) : Endpoint<ExportDeploymentRequest>
{
  public override void Configure()
  {
    Post(ExportDeploymentRequest.Route);
    AllowAnonymous();
    Summary(s => s.Summary = "Exportuje DataModely nasazení včetně jejich záznamů jako ZIP.");
  }

  public override async Task HandleAsync(ExportDeploymentRequest req, CancellationToken ct)
  {
    var result = await _mediator.Send(new ExportDeploymentQuery(req.DeploymentCode), ct);

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
        var definition = DataModelMapper.MapToDefinition(entry.Data.Model);
        var payload = new { Definition = definition, Records = entry.Data.Records };
        var zipEntry = archive.CreateEntry(entry.FileName, CompressionLevel.Optimal);
        await using var entryStream = zipEntry.Open();
        await JsonSerializer.SerializeAsync(entryStream, payload, ExportJsonOptions.Instance, ct);
      }
    }

    zipStream.Position = 0;
    await SendStreamAsync(zipStream, fileName: result.Value.ZipFileName, contentType: "application/zip", cancellation: ct);
  }
}
