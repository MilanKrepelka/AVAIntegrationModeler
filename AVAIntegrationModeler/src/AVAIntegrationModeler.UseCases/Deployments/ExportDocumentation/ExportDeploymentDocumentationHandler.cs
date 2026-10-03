using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Areas;
using AVAIntegrationModeler.UseCases.DataModels;
using AVAIntegrationModeler.UseCases.Documentation;
using AVAIntegrationModeler.UseCases.Export;

namespace AVAIntegrationModeler.UseCases.Deployments.ExportDocumentation;

/// <summary>
/// Handler pro <see cref="ExportDeploymentDocumentationQuery"/>.
/// Pro každý DataModel nasazení zavolá AVAPlace endpoint pro markdown dokumentaci
/// a na konci přidá dokument měsíčních rozdílů metadat. Výsledek je ZIP archív.
/// Struktura souborů: <c>{area_code}/models/{model_code}-{model_id}.md</c> a <c>ListOfChanges.md</c>.
/// </summary>
public class ExportDeploymentDocumentationHandler(
  IDeploymentsQueryService _deployments,
  IDataModelQueryService _models,
  IAreasQueryService _areas,
  IDocumentationQueryService _docs)
  : IQueryHandler<ExportDeploymentDocumentationQuery, Result<ExportResult<string>>>
{
  private const string BezOblasti = "bez-oblasti";

  public async Task<Result<ExportResult<string>>> Handle(
    ExportDeploymentDocumentationQuery request, CancellationToken ct)
  {
    DeploymentDTO deployment;
    try
    {
      deployment = await _deployments.GetDeployment(request.DeploymentCode, ct);
    }
    catch (NotFoundException)
    {
      return Result<ExportResult<string>>.NotFound();
    }

    var allModels = await _models.ListAsync(Datasource.Database);
    var deploymentModels = allModels
      .Where(m => deployment.DataModelIds.Contains(m.Id))
      .ToList();

    var areaList = await _areas.ListAsync(Datasource.Database);
    var areaCodesById = areaList.ToDictionary(a => a.Id, a => a.Code);

    var entries = new List<ExportEntry<string>>();

    foreach (var model in deploymentModels)
    {
      var areaCode = model.AreaId is Guid aId && areaCodesById.TryGetValue(aId, out var code)
        ? code
        : BezOblasti;

      var markdown = await _docs.GetDataModelMarkdownDocumentAsync(model.Id, request.Months, ct);
      var fileName = $"{areaCode}/models/{model.Code}-{model.Id}.md";
      entries.Add(new ExportEntry<string>(fileName, markdown));
    }

    var changesMarkdown = await _docs.GetMonthlyMetadataDifferencesDocumentAsync(request.Months, ct);
    entries.Add(new ExportEntry<string>("ListOfChanges.md", changesMarkdown));

    var zipName = $"deployment-{request.DeploymentCode}-docs.zip";
    return Result<ExportResult<string>>.Success(new ExportResult<string>(zipName, entries));
  }
}
