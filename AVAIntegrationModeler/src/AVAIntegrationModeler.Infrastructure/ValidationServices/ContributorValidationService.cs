using Ardalis.Result;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain;
using AVAIntegrationModeler.Domain.ContributorAggregate;

namespace AVAIntegrationModeler.Infrastructure.ValidationServices;

/// <summary>
/// Validační služba pro Contributor. Contributor nemá žádné cross-aggregate business pravidlo
/// (nemá kód ani jinou vlastnost vyžadující unikátnost), takže obě metody vždy vrací úspěch.
/// Existuje pro jednotnost s ostatními agregáty a jako místo pro budoucí pravidla.
/// </summary>
public class ContributorValidationService : IDomainEntityValidationService<Contributor>
{
  /// <inheritdoc/>
  public Task<Result> Validate(Datasource datasource, Contributor domainEntity, CancellationToken ct)
    => Task.FromResult(Result.Success());

  /// <inheritdoc/>
  public Task<Result> ValidateForCreate(Datasource datasource, Contributor domainEntity, CancellationToken ct)
    => Task.FromResult(Result.Success());
}
