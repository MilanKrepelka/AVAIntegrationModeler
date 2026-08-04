using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.DataModelRecords;
using AVAIntegrationModeler.UseCases.DataModels;
using AVAIntegrationModeler.UseCases.Export;

namespace AVAIntegrationModeler.UseCases.Deployments.Export;

/// <summary>
/// Handler pro <see cref="ExportDeploymentQuery"/>. Exportuje DataModely nasazení včetně jejich záznamů.
/// </summary>
public class ExportDeploymentHandler(
  IDeploymentsQueryService _deployments,
  IDataModelQueryService _models,
  IDataModelRecordQueryService _records)
  : IQueryHandler<ExportDeploymentQuery, Result<ExportResult<DeploymentDataModelExportEntryDTO>>>
{
  private static readonly char[] _invalidChars = Path.GetInvalidFileNameChars();

  public async Task<Result<ExportResult<DeploymentDataModelExportEntryDTO>>> Handle(
    ExportDeploymentQuery request, CancellationToken ct)
  {
    DeploymentDTO deployment;
    try
    {
      deployment = await _deployments.GetDeployment(request.DeploymentCode, ct);
    }
    catch (NotFoundException)
    {
      return Result<ExportResult<DeploymentDataModelExportEntryDTO>>.NotFound();
    }

    var allModels = await _models.ListAsync(Datasource.Database);
    var deploymentModels = allModels
      .Where(m => deployment.DataModelIds.Contains(m.Id))
      .ToList();

    var entries = new List<ExportEntry<DeploymentDataModelExportEntryDTO>>(deploymentModels.Count);
    foreach (var model in deploymentModels)
    {
      var modelRecords = await _records.ListAsync(Datasource.Database, model.Id, cancellationToken: ct);
      entries.Add(new ExportEntry<DeploymentDataModelExportEntryDTO>(
        SafeName(model.Code, model.Id) + ".json",
        new DeploymentDataModelExportEntryDTO(model, modelRecords)));
    }

    var zipName = $"deployment-{SafeName(deployment.Code, deployment.Id)}-export.zip";
    return Result.Success(new ExportResult<DeploymentDataModelExportEntryDTO>(zipName, entries));
  }

  private static string SafeName(string? raw, Guid fallback)
  {
    var name = string.IsNullOrWhiteSpace(raw) ? fallback.ToString() : raw;
    return string.Concat(name.Select(c => _invalidChars.Contains(c) ? '_' : c));
  }
}
