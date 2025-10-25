using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Contracts.DTO;
/// <summary>
/// Lehké shrnutí feature, vhodné pro zahrnutí do ScenarioDTO bez plného FeatureDTO.
/// Obsahuje pouze nejdůležitější metadata potřebná pro zobrazení v seznamu.
/// </summary>
public record FeatureSummaryDTO
{
  /// <summary>
  /// Jedinečný identifikátor feature.
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Technický kód feature.
  /// </summary>
  public string Code { get; init; } = string.Empty;
}

