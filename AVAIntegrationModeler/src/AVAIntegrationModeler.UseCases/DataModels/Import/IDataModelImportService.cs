using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.UseCases.DataModels.Import;

/// <summary>
/// Služba pro import (upsert) jednoho datového modelu z AVAPlace do lokální databáze,
/// včetně jeho DataModelRecordů.
/// </summary>
public interface IDataModelImportService
{
  /// <summary>
  /// Importuje datový model z AVAPlace do lokální databáze. Model je párován podle <see cref="DataModelDTO.Code"/>
  /// (pokud neexistuje, je vytvořen; pokud existuje, jsou přepsána jeho pole), jeho DataModelRecordy jsou párovány
  /// podle ExternalId. Metoda je idempotentní a lze ji bezpečně opakovaně spustit.
  /// Nevolá invalidaci cache — o to se stará volající handler.
  /// </summary>
  /// <param name="dto">Datový model načtený z AVAPlace</param>
  /// <param name="ct">Token pro zrušení operace</param>
  /// <returns>Id lokálně vytvořeného/aktualizovaného datového modelu</returns>
  Task<Result<Guid>> ImportModelAsync(DataModelDTO dto, CancellationToken ct = default);
}
