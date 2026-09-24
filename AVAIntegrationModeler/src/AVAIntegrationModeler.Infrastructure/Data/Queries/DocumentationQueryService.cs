using AVAIntegrationModeler.AVAPlace;
using AVAIntegrationModeler.UseCases.Documentation;

namespace AVAIntegrationModeler.Infrastructure.Data.Queries;

/// <summary>
/// Implementace <see cref="IDocumentationQueryService"/> delegující na AVAPlace DataService.
/// Dokumentace existuje pouze na straně AVAPlace — žádná lokální DB větev.
/// </summary>
public class DocumentationQueryService(IIntegrationDataProvider _integrationDataProvider)
  : IDocumentationQueryService
{
  /// <inheritdoc/>
  public Task<string> GetDataModelMarkdownDocumentAsync(Guid modelId, int months, CancellationToken ct = default)
    => _integrationDataProvider.GetDataModelMarkdownDocumentAsync(modelId, months, ct);

  /// <inheritdoc/>
  public Task<string> GetMonthlyMetadataDifferencesDocumentAsync(int months, CancellationToken ct = default)
    => _integrationDataProvider.GetMonthlyMetadataDifferencesDocumentAsync(months, ct);
}
