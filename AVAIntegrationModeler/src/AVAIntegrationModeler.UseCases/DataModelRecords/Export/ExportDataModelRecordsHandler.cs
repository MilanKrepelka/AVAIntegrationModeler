using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Areas;
using AVAIntegrationModeler.UseCases.DataModels;
using AVAIntegrationModeler.UseCases.Export;

namespace AVAIntegrationModeler.UseCases.DataModelRecords.Export;

/// <summary>
/// Handler pro <see cref="ExportDataModelRecordsQuery"/>. Exportuje vybrané záznamy DataModelů jako ZIP archív,
/// kde je každý záznam uložen na cestě <c>dataobjects/{Area.Code}/qd-{Model.Name}.json</c> odvozené
/// od DataModelu, k němuž záznam patří (záznamy stejného modelu tak sdílí stejnou cestu v archívu).
/// </summary>
public class ExportDataModelRecordsHandler(
  IDataModelRecordQueryService _query,
  IDataModelQueryService _models,
  IAreasQueryService _areas)
  : IQueryHandler<ExportDataModelRecordsQuery, Result<ExportResult<DataModelRecordDTO>>>
{
  private const string BezOblasti = "bez-oblasti";
  private static readonly char[] _invalidChars = Path.GetInvalidFileNameChars();

  public async Task<Result<ExportResult<DataModelRecordDTO>>> Handle(ExportDataModelRecordsQuery request, CancellationToken ct)
  {
    if (request.Datasource != Datasource.Database)
      return Result<ExportResult<DataModelRecordDTO>>.Invalid(
        new ValidationError("Datasource", "Export je dostupný pouze pro Database."));

    var models = await _models.ListAsync(Datasource.Database);
    var modelsById = models.ToDictionary(m => m.Id);
    var areas = await _areas.ListAsync(Datasource.Database);
    var areaCodesById = areas.ToDictionary(a => a.Id, a => a.Code);

    var entries = new List<ExportEntry<DataModelRecordDTO>>();
    foreach (var id in request.RecordIds)
    {
      var dto = await _query.GetByIdAsync(Datasource.Database, id, ct);
      if (dto is not null)
        entries.Add(new ExportEntry<DataModelRecordDTO>(BuildFileName(dto, modelsById, areaCodesById), dto));
    }

    return Result.Success(new ExportResult<DataModelRecordDTO>("records-export.zip", entries));
  }

  /// <summary>
  /// Sestaví cestu k souboru v ZIP archívu ve formátu <c>dataobjects/{Area.Code}/qd-{Model.Name}.json</c>,
  /// kde se oblast a název odvíjí od DataModelu, k němuž záznam patří (nikoli od samotného záznamu).
  /// Pokud DataModel nemá přiřazenou oblast, použije se adresář <c>bez-oblasti</c>.
  /// </summary>
  private static string BuildFileName(
    DataModelRecordDTO record,
    IReadOnlyDictionary<Guid, DataModelDTO> modelsById,
    IReadOnlyDictionary<Guid, string> areaCodesById)
  {
    var model = modelsById.GetValueOrDefault(record.ModelId);
    var areaCode = model?.AreaId is Guid areaId && areaCodesById.TryGetValue(areaId, out var code)
      ? code
      : BezOblasti;
    var modelName = model?.Name ?? record.ModelId.ToString();

    return $"dataobjects/{SafeName(areaCode, Guid.Empty)}/qd-{SafeName(modelName, record.ModelId)}.json";
  }

  private static string SafeName(string? raw, Guid fallback)
  {
    var name = string.IsNullOrWhiteSpace(raw) ? fallback.ToString() : raw;
    return string.Concat(name.Select(c => _invalidChars.Contains(c) ? '_' : c));
  }
}
