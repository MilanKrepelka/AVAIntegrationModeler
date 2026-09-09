using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Areas;
using AVAIntegrationModeler.UseCases.DataModels;
using AVAIntegrationModeler.UseCases.Export;

namespace AVAIntegrationModeler.UseCases.DataModelRecords.Export;

/// <summary>
/// Handler pro <see cref="ExportDataModelRecordsQuery"/>. Exportuje vybrané záznamy DataModelů jako ZIP archív,
/// kde jsou všechny vybrané záznamy náležící jednomu DataModelu seskupeny do jednoho souboru na cestě
/// <c>dataobjects/{Area.Code}/qd-{Model.Name}.json</c> odvozené od tohoto DataModelu.
/// </summary>
public class ExportDataModelRecordsHandler(
  IDataModelRecordQueryService _query,
  IDataModelQueryService _models,
  IAreasQueryService _areas)
  : IQueryHandler<ExportDataModelRecordsQuery, Result<ExportResult<List<DataModelRecordDTO>>>>
{
  private const string BezOblasti = "bez-oblasti";
  private static readonly char[] _invalidChars = Path.GetInvalidFileNameChars();

  public async Task<Result<ExportResult<List<DataModelRecordDTO>>>> Handle(ExportDataModelRecordsQuery request, CancellationToken ct)
  {
    if (request.Datasource != Datasource.Database)
      return Result<ExportResult<List<DataModelRecordDTO>>>.Invalid(
        new ValidationError("Datasource", "Export je dostupný pouze pro Database."));

    var models = await _models.ListAsync(Datasource.Database);
    var modelsById = models.ToDictionary(m => m.Id);
    var areas = await _areas.ListAsync(Datasource.Database);
    var areaCodesById = areas.ToDictionary(a => a.Id, a => a.Code);

    var records = new List<DataModelRecordDTO>();
    foreach (var id in request.RecordIds)
    {
      var dto = await _query.GetByIdAsync(Datasource.Database, id, ct);
      if (dto is not null)
        records.Add(dto);
    }

    var entries = records
      .GroupBy(r => r.ModelId)
      .Select(g => new ExportEntry<List<DataModelRecordDTO>>(
        BuildFileName(g.Key, modelsById, areaCodesById),
        g.Select(r => r with { Fields = r.Fields.OrderBy(f => f.Key, StringComparer.OrdinalIgnoreCase).ToList() }).ToList()));

    return Result.Success(new ExportResult<List<DataModelRecordDTO>>("records-export.zip", entries));
  }

  /// <summary>
  /// Sestaví cestu k souboru v ZIP archívu ve formátu <c>dataobjects/{Area.Code}/qd-{Model.Name}.json</c>
  /// pro DataModel, ke kterému seskupené záznamy patří.
  /// Pokud DataModel nemá přiřazenou oblast, použije se adresář <c>bez-oblasti</c>.
  /// </summary>
  private static string BuildFileName(
    Guid modelId,
    IReadOnlyDictionary<Guid, DataModelDTO> modelsById,
    IReadOnlyDictionary<Guid, string> areaCodesById)
  {
    var model = modelsById.GetValueOrDefault(modelId);
    var areaCode = model?.AreaId is Guid areaId && areaCodesById.TryGetValue(areaId, out var code)
      ? code
      : BezOblasti;
    var modelName = model?.Name ?? modelId.ToString();

    return $"dataobjects/{SafeName(areaCode, Guid.Empty)}/qd-{SafeName(modelName, modelId)}.json";
  }

  private static string SafeName(string? raw, Guid fallback)
  {
    var name = string.IsNullOrWhiteSpace(raw) ? fallback.ToString() : raw;
    return string.Concat(name.Select(c => _invalidChars.Contains(c) ? '_' : c));
  }
}
