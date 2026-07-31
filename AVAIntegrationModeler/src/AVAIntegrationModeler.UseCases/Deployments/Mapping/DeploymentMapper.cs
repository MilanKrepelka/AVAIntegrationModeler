using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.DeploymentAggregate;

namespace AVAIntegrationModeler.UseCases.Deployments.Mapping;

/// <summary>
/// Statická třída pro mapování mezi doménovým objektem nasazení a jeho DTO.
/// </summary>
public static class DeploymentMapper
{
  /// <summary>
  /// Mapuje <see cref="Deployment"/> na <see cref="DeploymentDTO"/>.
  /// </summary>
  public static DeploymentDTO? MapToDTO(Deployment? deployment)
  {
    if (deployment == default) return default;

    return new DeploymentDTO
    {
      Id = deployment.Id,
      Code = deployment.Code,
      Name = deployment.Name,
      DataModelIds = deployment.DataModels.Select(m => m.DataModelId).ToList()
    };
  }

  /// <summary>
  /// Mapuje <see cref="DeploymentDTO"/> na <see cref="Deployment"/>.
  /// </summary>
  public static Deployment? MapToEntity(DeploymentDTO? dto)
  {
    if (dto == default) return default;

    var deployment = new Deployment(dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id, dto.Code);
    if (!string.IsNullOrEmpty(dto.Name))
      deployment.SetName(dto.Name);
    foreach (var id in dto.DataModelIds)
      deployment.AddDataModel(id);
    return deployment;
  }
}
