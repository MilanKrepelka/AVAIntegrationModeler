using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.DataModelRecords.Xls.ImportFromXls;

/// <summary>
/// Příkaz pro import záznamů datového modelu z XLS souboru.
/// Existující záznamy (identifikované sloupcem Id) jsou aktualizovány, nové jsou vytvořeny.
/// </summary>
/// <param name="Datasource">Datový zdroj (pouze Database).</param>
/// <param name="ModelId">Id datového modelu, ke kterému záznamy patří.</param>
/// <param name="FileContent">Bajty XLSX souboru.</param>
public record ImportDataModelRecordsFromXlsCommand(Datasource Datasource, Guid ModelId, byte[] FileContent)
  : ICommand<Result<ImportDataModelRecordsXlsResult>>;
