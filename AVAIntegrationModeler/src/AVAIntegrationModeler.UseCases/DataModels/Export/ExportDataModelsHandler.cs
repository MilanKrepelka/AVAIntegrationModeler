using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Areas;
using AVAIntegrationModeler.UseCases.Export;

namespace AVAIntegrationModeler.UseCases.DataModels.Export;

/// <summary>
/// Handler pro <see cref="ExportDataModelsQuery"/>. Exportuje vybrané DataModely jako ZIP archív,
/// kde je každý DataModel uložen na cestě <c>datamodels/{Area.Code}/dm-{Model.Name}.json</c>.
/// </summary>
public class ExportDataModelsHandler(IDataModelQueryService _query, IAreasQueryService _areas)
  : IQueryHandler<ExportDataModelsQuery, Result<ExportResult<DataModelDTO>>>
{
  private const string BezOblasti = "bez-oblasti";
  private static readonly char[] _invalidChars = Path.GetInvalidFileNameChars();

  public async Task<Result<ExportResult<DataModelDTO>>> Handle(ExportDataModelsQuery request, CancellationToken ct)
  {
    if (request.Datasource != Datasource.Database)
      return Result<ExportResult<DataModelDTO>>.Invalid(
        new ValidationError("Datasource", "Export je dostupný pouze pro Database."));

    var all = await _query.ListAsync(Datasource.Database);
    var areas = await _areas.ListAsync(Datasource.Database);
    var areaCodesById = areas.ToDictionary(a => a.Id, a => a.Code);

    var entries = all
      .Where(m => request.ModelIds.Contains(m.Id))
      .Select(m => new ExportEntry<DataModelDTO>(BuildFileName(m, areaCodesById), m));

    return Result.Success(new ExportResult<DataModelDTO>("datamodels-export.zip", entries));
  }

  /// <summary>
  /// Sestaví cestu k souboru v ZIP archívu ve formátu <c>datamodels/{Area.Code}/dm-{Model.Name}.json</c>.
  /// Pokud DataModel nemá přiřazenou oblast, použije se adresář <c>bez-oblasti</c>.
  /// </summary>
  private static string BuildFileName(DataModelDTO model, IReadOnlyDictionary<Guid, string> areaCodesById)
  {
    var areaCode = model.AreaId is Guid areaId && areaCodesById.TryGetValue(areaId, out var code)
      ? code
      : BezOblasti;

    return $"datamodels/{SafeName(areaCode, Guid.Empty)}/dm-{SafeName(model.Name, model.Id)}.json";
  }

  private static string SafeName(string? raw, Guid fallback)
  {
    var name = string.IsNullOrWhiteSpace(raw) ? fallback.ToString() : raw;
    return string.Concat(name.Select(c => _invalidChars.Contains(c) ? '_' : c));
  }
}
