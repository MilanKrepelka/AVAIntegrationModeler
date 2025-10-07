﻿using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Web.ViewModels.List;

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
  public LocalizedValue Name { get; init; } = new();

  /// <summary>
  /// Lokalizovaný popis scénáře (např. v češtině a angličtině).
  /// </summary>
  public LocalizedValue Description { get; init; } = new();
  public List.FeatureViewModel InputFeature { get; set; } = List.FeatureViewModel.Empty;
  public List.FeatureViewModel OutputFeature { get; set; } = List.FeatureViewModel.Empty;
}
