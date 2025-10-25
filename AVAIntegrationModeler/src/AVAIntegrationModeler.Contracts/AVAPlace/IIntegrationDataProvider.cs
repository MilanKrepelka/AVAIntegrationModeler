using System.Threading;
using System.Threading.Tasks;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.AVAPlace;

/// <summary>
/// Poskytovatele data pro integrace.
/// </summary>
/// <summary>
public interface IIntegrationDataProvider 
{
  /// <summary>
  /// Asynchronně získá kolekci integračních scénářů.
  /// </summary>
  /// <remarks>Tato metoda získá seznam integračních scénářů, které mohou být použity pro konfiguraci nebo správu integrací v systému. Operace podporuje zrušení pomocí zadaného <see cref="CancellationToken"/>.</remarks>
  /// <param name="ct">Token pro zrušení operace.</param>
  /// <returns>Úloha reprezentující asynchronní operaci. Výsledek úlohy obsahuje <see cref="IEnumerable{T}"/> objektů <see cref="ScenarioDTO"/> reprezentujících integrační scénáře.</returns>
  Task<IEnumerable<ScenarioDTO>> GetIntegrationScenariosAsync(CancellationToken ct = default);

  // <summary>
  /// Asynchronně získá kolekci integračních featur.
  /// </summary>
  /// <remarks>Tato metoda získá seznam integračních featur, které mohou být použity pro konfiguraci nebo správu integrací v systému. Operace podporuje zrušení pomocí zadaného <see cref="CancellationToken"/>.</remarks>
  /// <param name="ct">Token pro zrušení operace.</param>
  /// <returns>Úloha reprezentující asynchronní operaci. Výsledek úlohy obsahuje <see cref="IEnumerable{T}"/> objektů <see cref="ScenarioDTO"/> reprezentujících integrační scénáře.</returns>
  Task<IEnumerable<FeatureDTO>> GetIntegrationFeaturesAsync(CancellationToken ct = default);
}
