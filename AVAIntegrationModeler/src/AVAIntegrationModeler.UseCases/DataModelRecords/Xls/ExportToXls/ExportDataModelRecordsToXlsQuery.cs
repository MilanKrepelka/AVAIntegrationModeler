using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.UseCases.DataModelRecords.Xls.ExportToXls;

/// <summary>
/// Dotaz pro export záznamů datového modelu do XLS souboru.
/// </summary>
/// <param name="Datasource">Datový zdroj.</param>
/// <param name="ModelId">Id datového modelu, jehož záznamy se exportují.</param>
public record ExportDataModelRecordsToXlsQuery(Datasource Datasource, Guid ModelId)
  : IQuery<Result<(byte[] Content, string FileName)>>;
