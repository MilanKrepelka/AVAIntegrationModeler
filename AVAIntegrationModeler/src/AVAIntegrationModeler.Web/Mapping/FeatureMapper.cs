using Ardalis.GuardClauses;
using ASOL.DataService.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Web.ViewModels.List;

namespace AVAIntegrationModeler.Web.Mapping;

/// <summary>
/// Implementuje mapování mezi datovým přenosovým objektem funkce (<see cref="FeatureDTO"/>) a jeho zobrazeními ve webovém rozhraní.
/// </summary>
public static class FeatureMapper
{
  /// <inheritdoc/>
  public static FeatureListViewModel MapToViewModel(FeatureDTO dto)
  {
    Guard.Against.Null(dto, $"{nameof(FeatureMapper)} - {nameof(dto)}");

    return new FeatureListViewModel
    {
      Id = dto.Id,
      Description = dto.Description,
      Name = dto.Name,
      IncludedFeatures = dto.IncludedFeatures,
      IncludedModels = dto.IncludedModels,
      Code = dto.Code
    };
  }
}

