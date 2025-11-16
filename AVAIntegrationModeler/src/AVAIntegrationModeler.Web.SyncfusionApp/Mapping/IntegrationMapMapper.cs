using Ardalis.GuardClauses;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;

namespace AVAIntegrationModeler.Web.Mapping;

/// <summary>
/// Statická třída pro mapování mezi datovým přenosovým objektem integrační mapy (<see cref="IntegrationMapDTO"/>) a jeho zobrazeními ve webovém rozhraní.
/// </summary>
public static class IntegrationMapMapper
{
  /// <summary>
  /// Mapuje <see cref="IntegrationMapDTO"/> na <see cref="IntegrationMapListViewModel"/>.
  /// </summary>
  /// <param name="dto">Zdrojový DTO objekt integrační mapy.</param>
  /// <returns><see cref="IntegrationMapListViewModel"/> nebo null, pokud je vstup null.</returns>
  public static IntegrationMapListViewModel? MapToIntegrationMapListViewModel(IntegrationMapDTO? dto)
  {
    if (dto == default) return default;

    IntegrationMapListViewModel result = new IntegrationMapListViewModel
    {
      //Area = dto.Area,
      //ScenarioCodes = dto.ScenarioCodes
    };
      
    return result;
  }

  /// <summary>
  /// Mapuje <see cref="IntegrationMapDTO"/> na <see cref="IntegrationMapListViewModel"/>.
  /// </summary>
  /// <param name="dto">Zdrojový DTO objekt integrační mapy.</param>
  /// <returns><see cref="IntegrationMapListViewModel"/> nebo null, pokud je vstup null.</returns>
  public static IntegrationMapListViewModel? MapToIntegrationMapSummaryListViewModel(IntegrationMapSummaryDTO dto)
  {
    Guard.Against.Null(dto, nameof(dto));

    IntegrationMapListViewModel result = new IntegrationMapListViewModel
    {
      //Area = dto.Area,
      //ScenarioCodes = dto.ScenarioCodes
    };

    return result;
  }


}

