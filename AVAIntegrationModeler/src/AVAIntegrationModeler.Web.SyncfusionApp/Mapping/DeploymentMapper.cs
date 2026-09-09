using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Web.SyncfusionApp.ViewModels.List;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Mapping;

/// <summary>
/// Statická třída pro mapování mezi <see cref="DeploymentDTO"/> a view modely.
/// </summary>
public static class DeploymentMapper
{
  /// <summary>
  /// Mapuje <see cref="DeploymentDTO"/> na <see cref="DeploymentListViewModel"/>.
  /// </summary>
  public static DeploymentListViewModel? MapToDeploymentListViewModel(DeploymentDTO? dto)
  {
    if (dto == default) return default;

    return new DeploymentListViewModel
    {
      Id = dto.Id,
      Code = dto.Code,
      Name = dto.Name,
      DataModelCount = dto.DataModelIds.Count,
      LastSaveDateTime = dto.LastSaveDateTime,
      LastDeploymentDateTime = dto.LastDeploymentDateTime
    };
  }
}
