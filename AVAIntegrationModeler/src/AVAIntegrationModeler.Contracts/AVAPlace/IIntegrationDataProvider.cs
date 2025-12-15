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
  Task<IEnumerable<ScenarioDTO>> GetScenarios(CancellationToken ct = default);

  /// <summary>
  /// Získá integrační scénář podle jeho identifikátoru.
  /// </summary>
  /// <param name="scenarioId">Identifikátor scénáře</param>
  /// <param name="ct"><see cref="CancellationToken"/></param>
  /// <returns>Integrační scénář</returns>
  Task<ScenarioDTO> GetScenario(Guid scenarioId, CancellationToken ct = default);

  /// <summary>
  /// Asynchronně získá <see cref="FeatureSummaryDTO"/> podle jeho identifikátoru.
  /// </summary>
  /// <param name="featureId">Identifikátor feature</param>
  /// <param name="ct">Token pro zrušení operace.</param>
  /// <returns><see cref="ScenarioDTO"/></returns>
  Task<FeatureSummaryDTO> GetFeatureSummary(Guid featureId, CancellationToken ct = default);
  /// <summary>
  /// Asynchronně získá <see cref="FeatureSummaryDTO"/> podle jeho code.
  /// </summary>
  /// <param name="featureCode">Code feature</param>
  /// <param name="ct">Token pro zrušení operace.</param>
  /// <returns><see cref="ScenarioDTO"/></returns>
  Task<FeatureDTO> GetFeature(string featureCode, CancellationToken ct = default);

  /// <summary>
  /// Asynchronně získá <see cref="FeatureSummaryDTO"/> podle jeho identifikátoru.
  /// </summary>
  /// <param name="featureId">Identifikátor feature</param>
  /// <param name="ct">Token pro zrušení operace.</param>
  /// <returns><see cref="ScenarioDTO"/></returns>
  
  Task<FeatureDTO> GetFeature(Guid featureId, CancellationToken ct = default);
  
  /// <summary>
  /// Asynchronně získá <see cref="FeatureSummaryDTO"/> podle jeho code.
  /// </summary>
  /// <param name="featureCode">Code feature</param>
  /// <param name="ct">Token pro zrušení operace.</param>
  /// <returns><see cref="ScenarioDTO"/></returns>
  Task<FeatureSummaryDTO> GetFeatureSummary(string featureCode, CancellationToken ct = default);

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
