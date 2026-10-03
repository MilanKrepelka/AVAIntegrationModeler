using AVAIntegrationModeler.Contracts.DTO;
using ClosedXML.Excel;

namespace AVAIntegrationModeler.UseCases.DataModelRecords.Xls;

/// <summary>
/// Interní služba pro sestavení a parsování XLS souborů pro záznamy datových modelů.
/// Formát souboru: řádek 1 = záhlaví (Id, ExternalId, poté abecedně řazené klíče polí).
/// Lokalizovaná pole jsou rozdělena na dva sloupce: {Klíč}_CZ a {Klíč}_EN.
/// </summary>
internal static class DataModelRecordXlsService
{
  internal const string ColId = "Id";
  internal const string ColExternalId = "ExternalId";
  internal const string SuffixCz = "_CZ";
  internal const string SuffixEn = "_EN";

  /// <summary>
  /// Sestaví XLS soubor ze seznamu záznamů datového modelu.
  /// </summary>
  /// <param name="records">Záznamy datového modelu.</param>
  /// <returns>Bajty XLSX souboru.</returns>
  internal static byte[] BuildXls(IEnumerable<DataModelRecordDTO> records)
  {
    var recordList = records.ToList();

    // Zjisti pro každý klíč, zda je lokalizovaný (stačí u jediného záznamu)
    var fieldLocalized = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
    foreach (var rec in recordList)
      foreach (var f in rec.Fields)
      {
        if (!fieldLocalized.TryGetValue(f.Key, out var curr))
          fieldLocalized[f.Key] = f.IsLocalized;
        else if (f.IsLocalized)
          fieldLocalized[f.Key] = true;
      }

    var sortedKeys = fieldLocalized.Keys.OrderBy(k => k, StringComparer.OrdinalIgnoreCase).ToList();

    using var workbook = new XLWorkbook();
    var ws = workbook.Worksheets.Add("Records");

    // Záhlaví
    var col = 1;
    ws.Cell(1, col++).Value = ColId;
    ws.Cell(1, col++).Value = ColExternalId;

    foreach (var key in sortedKeys)
    {
      if (fieldLocalized[key])
      {
        ws.Cell(1, col++).Value = $"{key}{SuffixCz}";
        ws.Cell(1, col++).Value = $"{key}{SuffixEn}";
      }
      else
      {
        ws.Cell(1, col++).Value = key;
      }
    }

    var headerRow = ws.Row(1);
    headerRow.Style.Font.Bold = true;
    ws.SheetView.FreezeRows(1);

    // Data
    var row = 2;
    foreach (var rec in recordList)
    {
      col = 1;
      ws.Cell(row, col++).Value = rec.Id == Guid.Empty ? "" : rec.Id.ToString();
      ws.Cell(row, col++).Value = rec.ExternalId ?? "";

      var fieldsByKey = rec.Fields.ToDictionary(f => f.Key, StringComparer.OrdinalIgnoreCase);

      foreach (var key in sortedKeys)
      {
        fieldsByKey.TryGetValue(key, out var field);
        if (fieldLocalized[key])
        {
          ws.Cell(row, col++).Value = field?.CzechValue ?? "";
          ws.Cell(row, col++).Value = field?.EnglishValue ?? "";
        }
        else
        {
          ws.Cell(row, col++).Value = field?.StringValue ?? "";
        }
      }

      row++;
    }

    ws.Columns().AdjustToContents();

    using var ms = new MemoryStream();
    workbook.SaveAs(ms);
    return ms.ToArray();
  }

  /// <summary>
  /// Parsuje XLS soubor a vrátí záznamy připravené k upsert operaci.
  /// </summary>
  /// <param name="content">Bajty XLSX souboru.</param>
  /// <param name="modelId">Id datového modelu, ke kterému záznamy patří.</param>
  /// <returns>Parsované záznamy a chyby parsování.</returns>
  internal static (List<ParsedXlsRecord> Records, List<XlsImportRowError> ParseErrors) ParseXls(
    byte[] content, Guid modelId)
  {
    var records = new List<ParsedXlsRecord>();
    var errors = new List<XlsImportRowError>();

    XLWorkbook workbook;
    try
    {
      workbook = new XLWorkbook(new MemoryStream(content));
    }
    catch
    {
      errors.Add(new XlsImportRowError(0, "Soubor není platný Excel (XLSX) soubor."));
      return (records, errors);
    }

    using (workbook)
    {
      var ws = workbook.Worksheets.FirstOrDefault();
      if (ws is null)
      {
        errors.Add(new XlsImportRowError(0, "Excel soubor neobsahuje žádný list."));
        return (records, errors);
      }

      var lastUsed = ws.LastCellUsed();
      if (lastUsed is null || lastUsed.Address.RowNumber < 2)
      {
        // prázdný soubor není chyba — žádné záznamy
        return (records, errors);
      }

      var lastCol = lastUsed.Address.ColumnNumber;
      var lastRow = lastUsed.Address.RowNumber;

      // Čti záhlaví
      var headers = new Dictionary<int, string>();
      for (var c = 1; c <= lastCol; c++)
      {
        var val = ws.Cell(1, c).GetString().Trim();
        if (!string.IsNullOrEmpty(val))
          headers[c] = val;
      }

      if (!headers.Values.Contains(ColId, StringComparer.OrdinalIgnoreCase) ||
          !headers.Values.Contains(ColExternalId, StringComparer.OrdinalIgnoreCase))
      {
        errors.Add(new XlsImportRowError(1,
          $"Záhlaví musí obsahovat sloupce '{ColId}' a '{ColExternalId}'."));
        return (records, errors);
      }

      var idCol = headers.First(kv => kv.Value.Equals(ColId, StringComparison.OrdinalIgnoreCase)).Key;
      var extIdCol = headers.First(kv => kv.Value.Equals(ColExternalId, StringComparison.OrdinalIgnoreCase)).Key;

      // Identifikuj typy sloupců
      var czCols = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
      var enCols = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
      var stringCols = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

      foreach (var (c, header) in headers)
      {
        if (c == idCol || c == extIdCol) continue;
        if (header.EndsWith(SuffixCz, StringComparison.OrdinalIgnoreCase))
          czCols[header[..^SuffixCz.Length]] = c;
        else if (header.EndsWith(SuffixEn, StringComparison.OrdinalIgnoreCase))
          enCols[header[..^SuffixEn.Length]] = c;
        else
          stringCols[header] = c;
      }

      var localizedKeys = czCols.Keys
        .Union(enCols.Keys, StringComparer.OrdinalIgnoreCase)
        .ToList();

      // Data řádky
      for (var r = 2; r <= lastRow; r++)
      {
        // Přeskoč prázdné řádky
        var isEmpty = true;
        for (var c = 1; c <= lastCol; c++)
          if (!ws.Cell(r, c).IsEmpty()) { isEmpty = false; break; }
        if (isEmpty) continue;

        var idStr = ws.Cell(r, idCol).GetString().Trim();
        var recordId = Guid.Empty;
        if (!string.IsNullOrEmpty(idStr) && !Guid.TryParse(idStr, out recordId))
        {
          errors.Add(new XlsImportRowError(r, $"Neplatná hodnota Id '{idStr}'."));
          continue;
        }

        var externalId = ws.Cell(r, extIdCol).GetString().Trim();
        var fields = new List<DataModelRecordFieldDTO>();

        foreach (var (key, c) in stringCols)
          fields.Add(new DataModelRecordFieldDTO { Key = key, StringValue = ws.Cell(r, c).GetString() });

        foreach (var key in localizedKeys)
        {
          var czVal = czCols.TryGetValue(key, out var czC) ? ws.Cell(r, czC).GetString() : null;
          var enVal = enCols.TryGetValue(key, out var enC) ? ws.Cell(r, enC).GetString() : null;
          fields.Add(new DataModelRecordFieldDTO
          {
            Key = key,
            IsLocalized = true,
            CzechValue = czVal,
            EnglishValue = enVal
          });
        }

        records.Add(new ParsedXlsRecord(recordId, modelId, externalId, fields));
      }
    }

    return (records, errors);
  }
}

/// <summary>
/// Parsovaný záznam z XLS souboru připravený k upsert operaci.
/// </summary>
/// <param name="Id">Id záznamu (Guid.Empty = nový záznam).</param>
/// <param name="ModelId">Id datového modelu.</param>
/// <param name="ExternalId">Vnější identifikátor záznamu.</param>
/// <param name="Fields">Pole záznamu.</param>
internal record ParsedXlsRecord(Guid Id, Guid ModelId, string ExternalId, List<DataModelRecordFieldDTO> Fields);
