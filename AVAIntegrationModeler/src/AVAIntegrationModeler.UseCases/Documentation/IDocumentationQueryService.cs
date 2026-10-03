namespace AVAIntegrationModeler.UseCases.Documentation;

/// <summary>
/// Služba pro načítání markdown dokumentace z AVAPlace DataService.
/// </summary>
public interface IDocumentationQueryService
{
  /// <summary>
  /// Vrátí markdown dokument pro daný datový model.
  /// </summary>
  /// <param name="modelId">Identifikátor datového modelu</param>
  /// <param name="months">Počet měsíců pro generování dokumentu</param>
  /// <param name="ct">Token pro zrušení operace</param>
  Task<string> GetDataModelMarkdownDocumentAsync(Guid modelId, int months, CancellationToken ct = default);

  /// <summary>
  /// Vrátí markdown dokument měsíčních rozdílů metadat.
  /// </summary>
  /// <param name="months">Počet měsíců pro generování dokumentu</param>
  /// <param name="ct">Token pro zrušení operace</param>
  Task<string> GetMonthlyMetadataDifferencesDocumentAsync(int months, CancellationToken ct = default);
}
