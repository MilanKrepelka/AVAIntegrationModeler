using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.API.DataModelRecords;

/// <summary>
/// Request pro export záznamů datového modelu do XLS souboru.
/// </summary>
public class ExportToXlsRequest
{
  /// <summary>Datový zdroj.</summary>
  public Datasource Datasource { get; set; } = Datasource.Database;

  /// <summary>Id datového modelu, jehož záznamy se exportují.</summary>
  public Guid ModelId { get; set; }
}
