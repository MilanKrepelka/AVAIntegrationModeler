using Ardalis.GuardClauses;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Web.ViewModels.List;

namespace AVAIntegrationModeler.Web.Mapping;

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
  public static ScenarioListViewModel? MapToScenarioListViewModel(ScenarioDTO? dto)
  {
    if (dto == default) return default;

    ScenarioListViewModel result = new ScenarioListViewModel
    {
      Id = dto.Id,
      Code = dto.Code,
      Name = dto.Name,
      Description = dto.Description,
      InputFeature = dto.InputFeatureSummary != null 
        ? MapFeatureSummaryToViewModel(dto.InputFeatureSummary)
        : FeatureViewModel.Empty,
      OutputFeature = dto.OutputFeatureSummary != null
        ? MapFeatureSummaryToViewModel(dto.OutputFeatureSummary)
        : FeatureViewModel.Empty
    };

    return result;
  }

  /// <summary>
  /// Mapuje <see cref="FeatureSummaryDTO"/> na <see cref="FeatureViewModel"/>.
  /// </summary>
  /// <param name="featureSummary">Souhrnné informace o feature.</param>
  /// <returns><see cref="FeatureViewModel"/>.</returns>
  private static FeatureViewModel MapFeatureSummaryToViewModel(FeatureSummaryDTO featureSummary)
  {
    return new FeatureViewModel
    {
      Id = featureSummary.Id,
      Code = featureSummary.Code,
      Name = new Domain.ValueObjects.LocalizedValue(), // prázdná hodnota, protože FeatureSummaryDTO neobsahuje Name
      Description = new Domain.ValueObjects.LocalizedValue() // prázdná hodnota
    };
  }
}

