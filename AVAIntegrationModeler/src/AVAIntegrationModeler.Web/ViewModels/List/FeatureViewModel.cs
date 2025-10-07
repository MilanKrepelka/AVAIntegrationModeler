using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Web.ViewModels.List;

/// <summary>
/// ViewModel pro Feature v Scenario přehledu.
/// </summary>
public class FeatureViewModel
{
  /// <summary>
  /// View Model představující prázdnou/nezadanou Feature.
  /// </summary>
  public static FeatureViewModel Empty => new FeatureViewModel
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
}
