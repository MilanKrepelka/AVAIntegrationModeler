using Ardalis.GuardClauses;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Mapping;

/// <summary>
/// Statická třída pro mapování mezi datovým přenosovým objektem scénáře (<see cref="ScenarioDTO"/>) a jeho zobrazeními ve webovém rozhraní.
/// </summary>
public static class ScenarioMapper
{
  /// <summary>
  /// Mapuje <see cref="ScenarioDTO"/> na <see cref="ScenarioListViewModel"/>.
  /// </summary>
  /// <param name="dto">Zdrojový DTO objekt scénáře.</param>
  /// <returns><see cref="ScenarioListViewModel"/> nebo null, pokud je vstup null.</returns>
  public static ScenarioListViewModel? MapToScenarioListViewModel(ScenarioDTO dto)
  {
    if (dto == default) return default;

    ScenarioListViewModel result = new ScenarioListViewModel
    {
      Id = dto.Id,
      Code = dto.Code,
      EnglishName = dto?.Name?.EnglishValue ?? string.Empty,
      EnglishDescription = dto?.Description?.EnglishValue ?? string.Empty,
      InputFeatureCode = dto?.InputFeatureSummary != null
        ? dto.InputFeatureSummary.Code
        : string.Empty,
      OutputFeatureCode = dto?.OutputFeatureSummary != null
        ? dto.OutputFeatureSummary.Code
        : string.Empty,
    };

    return result;
  }

  /// <summary>
  /// Mapuje <see cref="FeatureSummaryDTO"/> na <see cref="FeatureListViewModel"/>.
  /// </summary>
  /// <param name="featureSummary">Souhrnné informace o feature.</param>
  /// <returns><see cref="FeatureListViewModel"/>.</returns>
  private static FeatureListViewModel MapFeatureSummaryToViewModel(FeatureSummaryDTO featureSummary)
  {
    return new FeatureListViewModel
    {
      Id = featureSummary.Id,
      Code = featureSummary.Code,
      Name = new Domain.ValueObjects.LocalizedValue(), // prázdná hodnota, protože FeatureSummaryDTO neobsahuje Name
      Description = new Domain.ValueObjects.LocalizedValue() // prázdná hodnota
    };
  }
}

