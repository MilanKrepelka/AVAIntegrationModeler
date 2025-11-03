using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Web.ViewModels.List;

/// <summary>
/// ViewModel pro FeatureId v Scenario přehledu.
/// </summary>
public class FeatureListViewModel
{
  /// <summary>
  /// View Model představující prázdnou/nezadanou FeatureId.
  /// </summary>
  public static FeatureListViewModel Empty => new FeatureListViewModel
  {
    Id = Guid.Empty,
    Name = LocalizedValue.Empty,
    Description = LocalizedValue.Empty,
    Code = string.Empty
  };

  /// <summary>
  /// <see cref="FeatureDTO.Id"/>
  /// </summary>
  public Guid Id { get; set; } = Guid.Empty;

  /// <summary>
  /// <see cref="FeatureDTO.Code"/>
  /// </summary>
  public string Code { get; set; } = string.Empty;
  
  /// <summary>
  /// <see cref="FeatureDTO.Name"/>
  /// </summary>
  public LocalizedValue Name { get; set; } = LocalizedValue.Empty;

  /// <summary>
  /// <see cref="FeatureDTO.Description"/>
  /// </summary>
  public LocalizedValue Description { get; set; } = LocalizedValue.Empty;

  public string IncludedModelsText { get => string.Join(", ", IncludedModels.Select(m => m.DataModel.Code)); }
  public string IncludedFeaturesText { get => string.Join(", ", IncludedFeatures.Select(m => m.Feature.Code)); }

  public List<IncludedDataModelDTO> IncludedModels { get; set; } = new();
  public List<IncludedFeatureDTO> IncludedFeatures { get; set; } = new();

  /// <summary>
  /// Updates IncludedModelsText from IncludedModels DataModelSummaryDTO.Code values.
  /// </summary>
  
}
