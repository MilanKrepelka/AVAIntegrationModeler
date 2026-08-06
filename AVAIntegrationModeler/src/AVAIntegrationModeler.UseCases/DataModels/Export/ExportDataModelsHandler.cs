using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Areas;
using AVAIntegrationModeler.UseCases.DataModelRecords;
using AVAIntegrationModeler.UseCases.Export;

namespace AVAIntegrationModeler.UseCases.DataModels.Export;

/// <summary>
/// Handler pro <see cref="ExportDataModelsQuery"/>. Exportuje vybrané DataModely jako ZIP archív.
/// Pro každý DataModel vznikne definiční soubor na cestě <c>datamodels/{Area.Code}/dm-{Model.Name}.json</c>
/// a pokud má model záznamy, i soubor s jejich seznamem na cestě <c>dataobjects/{Area.Code}/qd-{Model.Name}.json</c>.
/// </summary>
public class ExportDataModelsHandler(
  IDataModelQueryService _query,
  IDataModelRecordQueryService _records,
  IAreasQueryService _areas)
  : IQueryHandler<ExportDataModelsQuery, Result<ExportResult<object>>>
{
  private const string BezOblasti = "bez-oblasti";
  private static readonly char[] _invalidChars = Path.GetInvalidFileNameChars();

  public async Task<Result<ExportResult<object>>> Handle(ExportDataModelsQuery request, CancellationToken ct)
  {
    if (request.Datasource != Datasource.Database)
      return Result<ExportResult<object>>.Invalid(
        new ValidationError("Datasource", "Export je dostupný pouze pro Database."));

    var all = await _query.ListAsync(Datasource.Database);
    var areas = await _areas.ListAsync(Datasource.Database);
    var areaCodesById = areas.ToDictionary(a => a.Id, a => a.Code);

    var models = all.Where(m => request.ModelIds.Contains(m.Id)).ToList();

    var entries = new List<ExportEntry<object>>();
    foreach (var model in models)
    {
      entries.Add(new ExportEntry<object>(BuildDefinitionFileName(model, areaCodesById), model));

      var modelRecords = (await _records.ListAsync(Datasource.Database, model.Id, cancellationToken: ct)).ToList();
      if (modelRecords.Count > 0)
        entries.Add(new ExportEntry<object>(BuildRecordsFileName(model, areaCodesById), modelRecords));
    }

    return Result.Success(new ExportResult<object>("datamodels-export.zip", entries));
  }

  /// <summary>
  /// Sestaví cestu k definičnímu souboru v ZIP archívu ve formátu <c>datamodels/{Area.Code}/dm-{Model.Name}.json</c>.
  /// Pokud DataModel nemá přiřazenou oblast, použije se adresář <c>bez-oblasti</c>.
  /// </summary>
  private static string BuildDefinitionFileName(DataModelDTO model, IReadOnlyDictionary<Guid, string> areaCodesById)
    => $"datamodels/{SafeName(AreaCode(model, areaCodesById), Guid.Empty)}/dm-{SafeName(model.Name, model.Id)}.json";

  /// <summary>
  /// Sestaví cestu k souboru se záznamy DataModelu v ZIP archívu ve formátu
  /// <c>dataobjects/{Area.Code}/qd-{Model.Name}.json</c>.
  /// Pokud DataModel nemá přiřazenou oblast, použije se adresář <c>bez-oblasti</c>.
  /// </summary>
  private static string BuildRecordsFileName(DataModelDTO model, IReadOnlyDictionary<Guid, string> areaCodesById)
    => $"dataobjects/{SafeName(AreaCode(model, areaCodesById), Guid.Empty)}/qd-{SafeName(model.Name, model.Id)}.json";

  private static string AreaCode(DataModelDTO model, IReadOnlyDictionary<Guid, string> areaCodesById)
    => model.AreaId is Guid areaId && areaCodesById.TryGetValue(areaId, out var code) ? code : BezOblasti;

  private static string SafeName(string? raw, Guid fallback)
  {
    var name = string.IsNullOrWhiteSpace(raw) ? fallback.ToString() : raw;
    return string.Concat(name.Select(c => _invalidChars.Contains(c) ? '_' : c));
  }
}
