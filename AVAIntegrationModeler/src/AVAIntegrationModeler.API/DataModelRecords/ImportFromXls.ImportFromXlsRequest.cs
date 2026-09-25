using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.API.DataModelRecords;

/// <summary>
/// Request pro import záznamů datového modelu z XLS souboru (multipart/form-data).
/// </summary>
public class ImportFromXlsRequest
{
  /// <summary>Datový zdroj.</summary>
  public Datasource Datasource { get; set; } = Datasource.Database;

  /// <summary>Id datového modelu, ke kterému záznamy patří.</summary>
  public Guid ModelId { get; set; }

  /// <summary>Nahrávaný XLSX soubor.</summary>
  public IFormFile File { get; set; } = default!;
}
