using Ardalis.GuardClauses;
using ASOL.DataService.Contracts;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Web.ViewModels.List;

namespace AVAIntegrationModeler.Web.Mapping;

/// <summary>
/// Implementuje mapování mezi datovým přenosovým objektem funkce (<see cref="FeatureDTO"/>) a jeho zobrazeními ve webovém rozhraní.
/// </summary>
public class ScenarioMapper :
  IViewModelMapper<ScenarioDTO, ViewModels.List.ScenarioListViewModel, ScenarioMapper>
{
  /// <inheritdoc/>
  public static void MapToViewModel(ScenarioDTO dto, out ScenarioListViewModel result)
  {
    Guard.Against.Null(dto, $"{nameof(FeatureMapper)} - {nameof(dto)}");

    result = new ScenarioListViewModel
    {
      Id = dto.Id,
      Description = dto.Description,
      Name = dto.Name,
      Code = dto.Code
      
    };
  }
}

