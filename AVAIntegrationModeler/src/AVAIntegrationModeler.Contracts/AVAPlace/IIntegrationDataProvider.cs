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
  Task<IEnumerable<ScenarioDTO>> GetScenariosAsync(CancellationToken ct = default);

  /// <summary>
  /// Asynchronně získá kolekci integračních featur.
  /// </summary>
  /// <remarks>Tato metoda získá seznam integračních featur, které mohou být použity pro konfiguraci nebo správu integrací v systému. Operace podporuje zrušení pomocí zadaného <see cref="CancellationToken"/>.</remarks>
  /// <param name="ct">Token pro zrušení operace.</param>
  /// <returns>Úloha reprezentující asynchronní operaci. Výsledek úlohy obsahuje <see cref="IEnumerable{T}"/> objektů <see cref="ScenarioDTO"/> reprezentujících integrační scénáře.</returns>
  Task<IEnumerable<FeatureSummaryDTO>> GetFeaturesSummaryAsync(CancellationToken ct = default);

  /// <summary>
  /// Asynchronně získá kolekci integračních featur.
  /// </summary>
  /// <remarks>Tato metoda získá seznam integračních featur, které mohou být použity pro konfiguraci nebo správu integrací v systému. Operace podporuje zrušení pomocí zadaného <see cref="CancellationToken"/>.</remarks>
  /// <param name="ct">Token pro zrušení operace.</param>
  /// <returns>Úloha reprezentující asynchronní operaci. Výsledek úlohy obsahuje <see cref="IEnumerable{T}"/> objektů <see cref="ScenarioDTO"/> reprezentujících integrační scénáře.</returns>
  Task<IEnumerable<FeatureDTO>> GetFeaturesAsync(CancellationToken ct = default);

  /// <summary>
  /// Asynchronně získá kolekci datových modelů.
  /// </summary>
  /// <remarks>Tato metoda získá seznam datových modelů, které mohou být použity pro konfiguraci nebo správu integrací v systému. Operace podporuje zrušení pomocí zadaného <see cref="CancellationToken"/>.</remarks>
  /// <param name="ct">Token pro zrušení operace.</param>
  /// <returns>Úloha reprezentující asynchronní operaci. Výsledek úlohy obsahuje <see cref="IEnumerable{T}"/> objektů <see cref="DataModelDTO"/> reprezentujících datové modely.</returns>
  Task<IEnumerable<DataModelDTO>> GetDataModelsAsync(CancellationToken ct = default);

  /// <summary>
  /// Asynchronně získá kolekci souhrnných informací o datových modelech.
  /// </summary>
  /// <remarks>Tato metoda získá seznam souhrnných informací o datových modelech, které mohou být použity pro zobrazení v seznamech nebo pro rychlý přehled. Operace podporuje zrušení pomocí zadaného <see cref="CancellationToken"/>.</remarks>
  /// <param name="ct">Token pro zrušení operace.</param>
  /// <returns>Úloha reprezentující asynchronní operaci. Výsledek úlohy obsahuje <see cref="IEnumerable{T}"/> objektů <see cref="DataModelSummaryDTO"/> reprezentujících souhrnné informace o datových modelech.</returns>
  Task<IEnumerable<DataModelSummaryDTO>> GetDataModelsSummaryAsync(CancellationToken ct = default);

  /// <summary>
  /// Asynchronně získá kolekci souhrnných informací o integračních mapách.
  /// </summary>
  /// <remarks>Tato metoda získá seznam souhrnných informací o integračních mapách, které mohou být použity pro zobrazení v seznamech nebo pro rychlý přehled. Operace podporuje zrušení pomocí zadaného <see cref="CancellationToken"/>.</remarks>
  /// <param name="ct">Token pro zrušení operace.</param>
  /// <returns>Úloha reprezentující asynchronní operaci. Výsledek úlohy obsahuje <see cref="IEnumerable{T}"/> objektů <see cref="IntegrationMapSummaryDTO"/> reprezentujících souhrnné informace o integračních mapách.</returns>
  Task<IEnumerable<IntegrationMapSummaryDTO>> GetIntegrationMapSummaryAsync(CancellationToken ct = default);
}
