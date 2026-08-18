using Ardalis.Result;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain;
using AVAIntegrationModeler.Domain.DataModelRecordAggregate;

namespace AVAIntegrationModeler.Infrastructure.ValidationServices;

/// <summary>
/// Validační služba pro DataModelRecord. ExternalId dnes nemá vynucenou unikátnost per model,
/// takže obě metody vždy vrací úspěch. Existuje pro jednotnost s ostatními agregáty a jako
/// místo pro budoucí pravidlo (např. unikátní ExternalId v rámci jednoho DataModelu).
/// </summary>
public class DataModelRecordValidationService : IDomainEntityValidationService<DataModelRecord>
{
  /// <inheritdoc/>
  public Task<Result> Validate(Datasource datasource, DataModelRecord domainEntity, CancellationToken ct)
    => Task.FromResult(Result.Success());

  /// <inheritdoc/>
  public Task<Result> ValidateForCreate(Datasource datasource, DataModelRecord domainEntity, CancellationToken ct)
    => Task.FromResult(Result.Success());
}
