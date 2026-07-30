using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Mapping;

/// <summary>
/// Statická třída pro mapování mezi <see cref="AreaDTO"/> a view modely.
/// </summary>
public static class AreaMapper
{
  /// <summary>
  /// Mapuje <see cref="AreaDTO"/> na <see cref="AreaListViewModel"/>.
  /// </summary>
  public static AreaListViewModel? MapToAreaListViewModel(AreaDTO? dto)
  {
    if (dto == default) return default;

    return new AreaListViewModel
    {
      Id = dto.Id,
      Code = dto.Code,
      Name = dto.Name
    };
  }
}
