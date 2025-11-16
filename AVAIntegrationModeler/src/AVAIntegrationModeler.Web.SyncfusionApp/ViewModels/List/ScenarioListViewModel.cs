using System.Runtime.CompilerServices;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;

/// <summary>
/// Třída představující ViewModel pro seznam scénářů.
/// </summary>
public class ScenarioListViewModel
{
  /// <summary>
  /// Jedinečný identifikátor scénáře.
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Kód scénáře, který slouží k jeho jednoznačné identifikaci v rámci systému.
  /// </summary>
  public string Code { get; init; } = string.Empty;

  /// <summary>
  /// Lokalizovaný název scénáře (např. v češtině a angličtině).
  /// </summary>
  public string EnglishName { get; init; } = string.Empty;
  

  /// <summary>
  /// Lokalizovaný popis scénáře (např. v češtině a angličtině).
  /// </summary>
  public string EnglishDescription { get; init; } = string.Empty;
  
  /// <summary>
  /// Vstupní feature scénáře.
  /// </summary>
  public string InputFeatureCode { get; set; } = string.Empty;

  /// <summary>
  /// Výstupní feature scénáře.
  /// </summary>
  public string OutputFeatureCode { get; set; } = string.Empty;

  /// <summary>
  /// Příznak, že se v Gridu bude zobrazovat detail <see cref="ScenarioListViewModel"/>
  /// </summary>
  public bool ShowDetails { get; set; } = false;
}
