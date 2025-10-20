using Ardalis.GuardClauses;
using ASOL.DataService.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Web.ViewModels.List;

namespace AVAIntegrationModeler.Web.Mapping;

/// <summary>
/// Implementuje mapování mezi datovým přenosovým objektem funkce (<see cref="FeatureDTO"/>) a jeho zobrazeními ve webovém rozhraní.
/// </summary>
public class FeatureMapper :
  IViewModelMapper<FeatureDTO, ViewModels.List.FeatureViewModel, FeatureMapper>
{
  /// <inheritdoc/>
  public static void MapToViewModel(FeatureDTO dto, out FeatureViewModel result)
  {
    Guard.Against.Null(dto, $"{nameof(FeatureMapper)} - {nameof(dto)}");

    result = new FeatureViewModel
    {
      Id = dto.Id,
      Description = dto.Description,
      Name = dto.Name,
    };
  }
}

