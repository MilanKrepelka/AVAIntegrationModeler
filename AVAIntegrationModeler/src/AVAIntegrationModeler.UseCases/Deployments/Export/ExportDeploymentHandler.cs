using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Areas;
using AVAIntegrationModeler.UseCases.DataModelRecords;
using AVAIntegrationModeler.UseCases.DataModels;
using AVAIntegrationModeler.UseCases.Export;

namespace AVAIntegrationModeler.UseCases.Deployments.Export;

/// <summary>
/// Handler pro <see cref="ExportDeploymentQuery"/>. Exportuje DataModely nasazení jako ZIP archív.
/// Pro každý DataModel vznikne definiční soubor na cestě <c>datamodels/{Area.Code}/dm-{Model.Code}.json</c>
/// a pokud má model záznamy, i soubor s jejich seznamem na cestě <c>dataobjects/{Area.Code}/qd-{Model.Code}.json</c>.
/// </summary>
public class ExportDeploymentHandler(
  IDeploymentsQueryService _deployments,
  IDataModelQueryService _models,
  IDataModelRecordQueryService _records,
  IAreasQueryService _areas)
  : IQueryHandler<ExportDeploymentQuery, Result<ExportResult<object>>>
{
  private const string BezOblasti = "bez-oblasti";
  private static readonly char[] _invalidChars = Path.GetInvalidFileNameChars();

  public async Task<Result<ExportResult<object>>> Handle(
    ExportDeploymentQuery request, CancellationToken ct)
  {
    DeploymentDTO deployment;
    try
    {
      deployment = await _deployments.GetDeployment(request.DeploymentCode, ct);
    }
    catch (NotFoundException)
    {
      return Result<ExportResult<object>>.NotFound();
    }

    var allModels = await _models.ListAsync(Datasource.Database);
    var deploymentModels = allModels
      .Where(m => deployment.DataModelIds.Contains(m.Id))
      .ToList();

    var areas = await _areas.ListAsync(Datasource.Database);
    var areaCodesById = areas.ToDictionary(a => a.Id, a => a.Code);

    var entries = new List<ExportEntry<object>>();
    foreach (var model in deploymentModels)
    {
      var sortedModel = model with { Fields = SortFieldsByName(model.Fields) };
      entries.Add(new ExportEntry<object>(BuildDefinitionFileName(sortedModel, areaCodesById), sortedModel));

      var modelRecords = (await _records.ListAsync(Datasource.Database, model.Id, cancellationToken: ct))
        .Select(r => r with { Fields = r.Fields.OrderBy(f => f.Key, StringComparer.OrdinalIgnoreCase).ToList() })
        .ToList();
      if (modelRecords.Count > 0)
        entries.Add(new ExportEntry<object>(BuildRecordsFileName(sortedModel, areaCodesById), modelRecords));
    }

    var zipName = $"deployment-{SafeName(deployment.Code, deployment.Id)}-export.zip";
    return Result.Success(new ExportResult<object>(zipName, entries));
  }

  /// <summary>
  /// Sestaví cestu k definičnímu souboru v ZIP archívu ve formátu <c>datamodels/{Area.Code}/dm-{Model.Name}.json</c>.
  /// Pokud DataModel nemá přiřazenou oblast, použije se adresář <c>bez-oblasti</c>.
  /// </summary>
  private static string BuildDefinitionFileName(DataModelDTO model, IReadOnlyDictionary<Guid, string> areaCodesById)
    => $"datamodels/{SafeName(AreaCode(model, areaCodesById), Guid.Empty)}/dm-{SafeName(model.Code, model.Id)}.json";

  /// <summary>
  /// Sestaví cestu k souboru se záznamy DataModelu v ZIP archívu ve formátu
  /// <c>dataobjects/{Area.Code}/qd-{Model.Code}.json</c>.
  /// Pokud DataModel nemá přiřazenou oblast, použije se adresář <c>bez-oblasti</c>.
  /// </summary>
  private static string BuildRecordsFileName(DataModelDTO model, IReadOnlyDictionary<Guid, string> areaCodesById)
    => $"dataobjects/{SafeName(AreaCode(model, areaCodesById), Guid.Empty)}/qd-{SafeName(model.Code, model.Id)}.json";

  private static string AreaCode(DataModelDTO model, IReadOnlyDictionary<Guid, string> areaCodesById)
    => model.AreaId is Guid areaId && areaCodesById.TryGetValue(areaId, out var code) ? code : BezOblasti;

  /// <summary>
  /// Vrátí pole datového modelu seřazená abecedně (case-insensitive) dle Name.
  /// </summary>
  private static List<DataModelFieldDTO> SortFieldsByName(List<DataModelFieldDTO> fields)
    => fields.OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase).ToList();

  private static string SafeName(string? raw, Guid fallback)
  {
    var name = string.IsNullOrWhiteSpace(raw) ? fallback.ToString() : raw;
    return string.Concat(name.Select(c => _invalidChars.Contains(c) ? '_' : c));
  }
}
