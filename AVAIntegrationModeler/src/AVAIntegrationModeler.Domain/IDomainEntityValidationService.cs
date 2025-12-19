using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Domain.ScenarioAggregate;

namespace AVAIntegrationModeler.Domain;

/// <summary>
/// Rozhraní pro službu validace doménové entyty. Obsahuje metody pro validaci podle business pravidel.
/// </summary>
/// <remarks>Jsou tady pravdidla typu, že nemůžu přidat scenario s Code, který už existuje. Ale nejsou tady pravidla typu, že Code nesmí být null</remarks>
public interface IDomainEntityValidationService<T> where T : class
{
  /// <summary>
  /// Validuje <T> podle business pravidel (lokální i cross-aggregate).
  /// Vrací Result s typizovanými chybami.
  /// </summary>
  /// <param name="domainEntity">Doménová entita k validaci</param>
  /// <param name="ct"><see cref="CancellationToken"/></param>
  /// <returns>Result s chybami nebo úspěšný</returns>
  Task<Result> Validate(Datasource datasource, T domainEntity, CancellationToken ct);

  /// <summary>
  /// Validuje <T> podle business pravidel (lokální i cross-aggregate). Typicky nevložíme entitu, která už existuje. Buď se stejným Id nebo Code
  /// Vrací Result s typizovanými chybami.
  /// </summary>
  /// <param name="domainEntity">Doménová entita k validaci</param>
  /// <param name="ct"><see cref="CancellationToken"/></param>
  /// <returns>Result s chybami nebo úspěšný</returns>
  Task<Result> ValidateForCreate(Datasource datasource, T domainEntity, CancellationToken ct);

}
