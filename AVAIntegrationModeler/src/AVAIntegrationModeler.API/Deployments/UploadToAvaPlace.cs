using System.IO;
using System.Text.Json;
using AVAIntegrationModeler.API.Serialization;
using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.AVAPlace.Mapping;
using AVAIntegrationModeler.Contracts.Deployments;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Contracts.Options;
using AVAIntegrationModeler.UseCases.Deployments.Export;
using Microsoft.Extensions.Options;

namespace AVAIntegrationModeler.API.Deployments;

/// <summary>
/// Nahraje DataModely a záznamy (DataModelRecordy) nasazení do AVAPlace.
/// Tok: CreateMetadataVersion → ImportDataModel pro každý model → ImportUnifiedData pro záznamy každého modelu.
/// </summary>
public class UploadDeploymentToAvaPlaceEndpoint(
    IMediator _mediator,
    IOptions<AVAPlaceOptions> _avaPlaceOptions)
    : Endpoint<UploadDeploymentToAvaPlaceRequest, UploadDeploymentToAvaPlaceResult>
{
  public override void Configure()
  {
    Post(UploadDeploymentToAvaPlaceRequest.Route);
    AllowAnonymous();
    Summary(s => s.Summary = "Nahraje DataModely a záznamy nasazení do AVAPlace.");
  }

  public override async Task HandleAsync(UploadDeploymentToAvaPlaceRequest req, CancellationToken ct)
  {
    var exportResult = await _mediator.Send(new ExportDeploymentQuery(req.DeploymentCode), ct);

    if (exportResult.Status == ResultStatus.NotFound)
    {
      await SendNotFoundAsync(ct);
      return;
    }

    if (!exportResult.IsSuccess)
    {
      await SendErrorsAsync(400, ct);
      return;
    }

    string versionCode = string.Empty;
    int modelsImported = 0;
    int recordGroupsImported = 0;

    await ServiceRuntimeTenantContext.ExecuteInContextAsync<ICustomDataServiceClient>(
      HttpContext.RequestServices, _avaPlaceOptions.Value.TenantId, async client =>
      {
        versionCode = await client.CreateMetadataVersionAsync(ct);

        string? currentModelCode = null;

        foreach (var entry in exportResult.Value.Entries)
        {
          if (entry.Data is DataModelDTO modelDto)
          {
            currentModelCode = modelDto.Code;
            var definition = DataModelMapper.MapToDefinition(modelDto);
            using var modelStream = SerializeToUtf8Stream(definition);
            await client.ImportDataModelAsync(versionCode, allowUpdate: true, modelStream, ct);
            modelsImported++;
          }
          else if (entry.Data is List<DataModelRecordDTO> records && currentModelCode is not null)
          {
            using var recordStream = SerializeToUtf8Stream(records);
            await client.ImportUnifiedDataAsync(currentModelCode, versionCode, allowUpdate: true, allowChangeExternalId: true, recordStream, ct);
            recordGroupsImported++;
          }
        }
      });

    await SendAsync(
      new UploadDeploymentToAvaPlaceResult
      {
        VersionCode = versionCode,
        ModelsImported = modelsImported,
        RecordGroupsImported = recordGroupsImported
      },
      200, ct);
  }

  private static MemoryStream SerializeToUtf8Stream<T>(T data)
  {
    var bytes = JsonSerializer.SerializeToUtf8Bytes(data, ExportJsonOptions.Instance);
    return new MemoryStream(bytes);
  }
}
