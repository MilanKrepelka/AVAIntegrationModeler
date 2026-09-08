using AVAIntegrationModeler.Contracts.DataModels;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.DataModels.Compare;

/// <summary>
/// Sdílená logika pro hluboké porovnání DataModelu mezi databází a AVAPlace.
/// </summary>
internal static class DataModelComparer
{
  /// <summary>
  /// Sestaví kompletní porovnání pro jeden DataModel.
  /// </summary>
  internal static DataModelComparisonDTO BuildComparison(Guid id, DataModelDTO? db, DataModelDTO? ava)
  {
    var code = db?.Code ?? ava?.Code ?? string.Empty;
    var name = db?.Name ?? ava?.Name ?? string.Empty;
    bool existsInDb = db is not null;
    bool existsInAva = ava is not null;

    var propertyDiffs = (existsInDb && existsInAva)
      ? CompareModelProperties(db!, ava!)
      : new List<DataModelPropertyDiffDTO>();

    var modelStatus = (existsInDb, existsInAva) switch
    {
      (true, false) => ComparisonStatus.OnlyInDatabase,
      (false, true) => ComparisonStatus.OnlyInAvaPlace,
      _ => propertyDiffs.Count > 0 ? ComparisonStatus.Different : ComparisonStatus.Same
    };

    var fieldComparisons = CompareFields(
      db?.Fields ?? new List<DataModelFieldDTO>(),
      ava?.Fields ?? new List<DataModelFieldDTO>());

    bool hasDiffs = modelStatus != ComparisonStatus.Same
      || fieldComparisons.Any(f => f.Status != ComparisonStatus.Same);

    return new DataModelComparisonDTO
    {
      DataModelId = id,
      Code = code,
      Name = name,
      ExistsInDatabase = existsInDb,
      ExistsInAvaPlace = existsInAva,
      ModelStatus = modelStatus,
      HasDifferences = hasDiffs,
      PropertyDiffs = propertyDiffs,
      FieldComparisons = fieldComparisons
    };
  }

  internal static List<DataModelPropertyDiffDTO> CompareModelProperties(DataModelDTO db, DataModelDTO ava)
  {
    var diffs = new List<DataModelPropertyDiffDTO>();
    void Add(string name, string? dbVal, string? avaVal)
    {
      if (dbVal != avaVal) diffs.Add(new DataModelPropertyDiffDTO(name, dbVal, avaVal));
    }
    Add("Code", db.Code, ava.Code);
    Add("Name", db.Name, ava.Name);
    Add("Description", db.Description, ava.Description);
    Add("IsAggregateRoot", db.IsAggregateRoot.ToString(), ava.IsAggregateRoot.ToString());
    // Notes a AreaId se neporovnávají — AVAPlace tyto atributy neposkytuje
    return diffs;
  }

  internal static List<DataModelFieldComparisonDTO> CompareFields(
    List<DataModelFieldDTO> dbFields, List<DataModelFieldDTO> avaFields)
  {
    // Pole se párují podle Name (case-insensitive) — IDs polí nejsou shodná mezi zdroji
    var dbByName = dbFields
      .GroupBy(f => f.Name, StringComparer.OrdinalIgnoreCase)
      .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
    var avaByName = avaFields
      .GroupBy(f => f.Name, StringComparer.OrdinalIgnoreCase)
      .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
    var allNames = dbByName.Keys.Union(avaByName.Keys, StringComparer.OrdinalIgnoreCase);
    var result = new List<DataModelFieldComparisonDTO>();

    foreach (var name in allNames)
    {
      dbByName.TryGetValue(name, out var dbField);
      avaByName.TryGetValue(name, out var avaField);

      List<DataModelPropertyDiffDTO> diffs;
      ComparisonStatus status;
      if (dbField is not null && avaField is not null)
      {
        diffs = CompareFieldProperties(dbField, avaField);
        status = diffs.Count > 0 ? ComparisonStatus.Different : ComparisonStatus.Same;
      }
      else if (dbField is not null)
      {
        status = ComparisonStatus.OnlyInDatabase;
        diffs = new List<DataModelPropertyDiffDTO>();
      }
      else
      {
        status = ComparisonStatus.OnlyInAvaPlace;
        diffs = new List<DataModelPropertyDiffDTO>();
      }

      result.Add(new DataModelFieldComparisonDTO
      {
        FieldId = dbField?.Id ?? avaField?.Id ?? Guid.Empty,
        FieldName = dbField?.Name ?? avaField?.Name ?? string.Empty,
        Status = status,
        DatabaseField = dbField,
        AvaPlaceField = avaField,
        PropertyDiffs = diffs
      });
    }

    return result.OrderBy(f => f.FieldName, StringComparer.OrdinalIgnoreCase).ToList();
  }

  internal static List<DataModelPropertyDiffDTO> CompareFieldProperties(DataModelFieldDTO db, DataModelFieldDTO ava)
  {
    var diffs = new List<DataModelPropertyDiffDTO>();
    void Add(string name, string? dbVal, string? avaVal)
    {
      if (dbVal != avaVal) diffs.Add(new DataModelPropertyDiffDTO(name, dbVal, avaVal));
    }
    Add("Name", db.Name, ava.Name);
    Add("Label", db.Label, ava.Label);
    Add("Description", db.Description, ava.Description);
    Add("IsPublishedForLookup", db.IsPublishedForLookup.ToString(), ava.IsPublishedForLookup.ToString());
    Add("IsCollection", db.IsCollection.ToString(), ava.IsCollection.ToString());
    Add("IsLocalized", db.IsLocalized.ToString(), ava.IsLocalized.ToString());
    Add("IsNullable", db.IsNullable.ToString(), ava.IsNullable.ToString());
    Add("FieldType", db.FieldType.ToString(), ava.FieldType.ToString());

    var dbRefs = string.Join(", ", (db.ReferencedEntityTypeIds ?? new List<Guid>()).OrderBy(x => x));
    var avaRefs = string.Join(", ", (ava.ReferencedEntityTypeIds ?? new List<Guid>()).OrderBy(x => x));
    Add("ReferencedEntityTypeIds", dbRefs.Length > 0 ? dbRefs : null, avaRefs.Length > 0 ? avaRefs : null);

    Add("Expression.Value", db.Expression?.Value, ava.Expression?.Value);
    Add("Expression.Order", db.Expression?.Order.ToString(), ava.Expression?.Order.ToString());
    return diffs;
  }
}
