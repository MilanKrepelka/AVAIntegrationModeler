using System.Data.SqlTypes;
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
    /// Získá integrační scénář podle jeho kódu.
    /// </summary>
    /// <param name="scenarioCode">Kód scénáře</param>
    /// <param name="cancelationToken"><see cref="CancellationToken"/></param>
    /// <returns>Integrační scénář</returns>
    Task<ScenarioDTO> GetScenario(string scenarioCode, CancellationToken cancelationToken = default);

  /// <summary>
  /// Vytvoří nový integrační scénář v Data
  /// </summary>
  /// <param name="scenario">Integrační scénář</param>
  /// <param name="cancelationToken"><see cref="CancellationToken"/></param>
  /// <returns>Příznak, že scénář byl vložen do integrační služby</returns>
  Task<bool> CreateScenario(ScenarioDTO scenario, CancellationToken cancelationToken = default);

  /// <summary>
  /// Aktualizuje integrační scénář v DataService
  /// </summary>
  /// <param name="scenario">Integrační scénář</param>
  /// <param name="cancelationToken"><see cref="CancellationToken"/></param>
  /// <returns>Příznak, že scénář byl aktualizován v integrační službě</returns>
  Task<bool> UpdateScenario(ScenarioDTO scenario, CancellationToken cancelationToken = default);

  /// <summary>
  /// Smaže integrační scénář v DataService
  /// </summary>
  /// <param name="scenarioCode">Kód integračního scénáře</param>
  /// <param name="cancelationToken"><see cref="CancellationToken"/></param>
  /// <remarks>Používáme Code, protože DataService zatím nemá GUIDy</remarks>
  /// <returns>Příznak, že scénář byl smazán v integrační službě</returns>
  Task<bool> DeleteScenario(string scenarioCode, CancellationToken cancelationToken = default);


  /// <summary>
  /// Asynchronně získá <see cref="FeatureSummaryDTO"/> podle jeho identifikátoru.
  /// </summary>
  /// <param name="featureId">Identifikátor feature</param>
  /// <param name="cancelationToken">Token pro zrušení operace.</param>
  /// <returns><see cref="ScenarioDTO"/></returns>
  Task<FeatureSummaryDTO> GetFeatureSummary(Guid featureId, CancellationToken cancelationToken = default);
    /// <summary>
    /// Asynchronně získá <see cref="FeatureSummaryDTO"/> podle jeho code.
    /// </summary>
    /// <param name="featureCode">Code feature</param>
    /// <param name="cancelationToken">Token pro zrušení operace.</param>
    /// <returns><see cref="ScenarioDTO"/></returns>
    Task<FeatureDTO> GetFeature(string featureCode, CancellationToken cancelationToken = default);

    /// <summary>
    /// Asynchronně získá <see cref="FeatureSummaryDTO"/> podle jeho identifikátoru.
    /// </summary>
    /// <param name="featureId">Identifikátor feature</param>
    /// <param name="cancelationToken">Token pro zrušení operace.</param>
    /// <returns><see cref="ScenarioDTO"/></returns>

    Task<FeatureDTO> GetFeature(Guid featureId, CancellationToken cancelationToken = default);

    /// <summary>
    /// Asynchronně získá <see cref="FeatureSummaryDTO"/> podle jeho code.
    /// </summary>
    /// <param name="featureCode">Code feature</param>
    /// <param name="cancelationToken">Token pro zrušení operace.</param>
    /// <returns><see cref="ScenarioDTO"/></returns>
    Task<FeatureSummaryDTO> GetFeatureSummary(string featureCode, CancellationToken cancelationToken = default);

    /// <summary>
    /// Asynchronně získá kolekci integračních featur.
    /// </summary>
    /// <remarks>Tato metoda získá seznam integračních featur, které mohou být použity pro konfiguraci nebo správu integrací v systému. Operace podporuje zrušení pomocí zadaného <see cref="CancellationToken"/>.</remarks>
    /// <param name="cancelationToken">Token pro zrušení operace.</param>
    /// <returns>Úloha reprezentující asynchronní operaci. Výsledek úlohy obsahuje <see cref="IEnumerable{T}"/> objektů <see cref="ScenarioDTO"/> reprezentujících integrační scénáře.</returns>
    Task<IEnumerable<FeatureSummaryDTO>> GetFeaturesSummaryAsync(CancellationToken cancelationToken = default);

    /// <summary>
    /// Asynchronně získá kolekci integračních featur.
    /// </summary>
    /// <remarks>Tato metoda získá seznam integračních featur, které mohou být použity pro konfiguraci nebo správu integrací v systému. Operace podporuje zrušení pomocí zadaného <see cref="CancellationToken"/>.</remarks>
    /// <param name="cancelationToken">Token pro zrušení operace.</param>
    /// <returns>Úloha reprezentující asynchronní operaci. Výsledek úlohy obsahuje <see cref="IEnumerable{T}"/> objektů <see cref="ScenarioDTO"/> reprezentujících integrační scénáře.</returns>
    Task<IEnumerable<FeatureDTO>> GetFeaturesAsync(CancellationToken cancelationToken = default);

    /// <summary>
    /// Asynchronně získá kolekci datových modelů.
    /// </summary>
    /// <remarks>Tato metoda získá seznam datových modelů, které mohou být použity pro konfiguraci nebo správu integrací v systému. Operace podporuje zrušení pomocí zadaného <see cref="CancellationToken"/>.</remarks>
    /// <param name="cancelationToken">Token pro zrušení operace.</param>
    /// <returns>Úloha reprezentující asynchronní operaci. Výsledek úlohy obsahuje <see cref="IEnumerable{T}"/> objektů <see cref="DataModelDTO"/> reprezentujících datové modely.</returns>
    Task<IEnumerable<DataModelDTO>> GetDataModelsAsync(CancellationToken cancelationToken = default);

    /// <summary>
    /// Asynchronně získá kolekci souhrnných informací o datových modelech.
    /// </summary>
    /// <remarks>Tato metoda získá seznam souhrnných informací o datových modelech, které mohou být použity pro zobrazení v seznamech nebo pro rychlý přehled. Operace podporuje zrušení pomocí zadaného <see cref="CancellationToken"/>.</remarks>
    /// <param name="cancelationToken">Token pro zrušení operace.</param>
    /// <returns>Úloha reprezentující asynchronní operaci. Výsledek úlohy obsahuje <see cref="IEnumerable{T}"/> objektů <see cref="DataModelSummaryDTO"/> reprezentujících souhrnné informace o datových modelech.</returns>
    Task<IEnumerable<DataModelSummaryDTO>> GetDataModelsSummaryAsync(CancellationToken cancelationToken = default);

    /// <summary>
    /// Asynchronně získá kolekci souhrnných informací o integračních mapách.
    /// </summary>
    /// <remarks>Tato metoda získá seznam souhrnných informací o integračních mapách, které mohou být použity pro zobrazení v seznamech nebo pro rychlý přehled. Operace podporuje zrušení pomocí zadaného <see cref="CancellationToken"/>.</remarks>
    /// <param name="cancelationToken">Token pro zrušení operace.</param>
    /// <returns>Úloha reprezentující asynchronní operaci. Výsledek úlohy obsahuje <see cref="IEnumerable{T}"/> objektů <see cref="IntegrationMapSummaryDTO"/> reprezentujících souhrnné informace o integračních mapách.</returns>
    Task<IEnumerable<IntegrationMapSummaryDTO>> GetIntegrationMapSummaryAsync(CancellationToken cancelationToken = default);

    /// <summary>
    /// Asynchronně získá záznamy datového modelu.
    /// </summary>
    Task<IEnumerable<DataModelRecordDTO>> GetDataModelRecordsAsync(Guid modelId, CancellationToken cancelationToken = default);

    /// <summary>
    /// Asynchronně získá datový model podle jeho identifikátoru z AVAPlace.
    /// Protože ASOL DataService neposkytuje endpoint pro načtení modelu dle Id,
    /// načte kompletní seznam a filtruje v paměti.
    /// </summary>
    /// <param name="modelId">Identifikátor datového modelu v AVAPlace.</param>
    /// <param name="cancelationToken">Token pro zrušení operace.</param>
    /// <returns>DataModelDTO nebo null pokud model neexistuje.</returns>
    Task<DataModelDTO?> GetDataModelByIdAsync(Guid modelId, CancellationToken cancelationToken = default);
}
