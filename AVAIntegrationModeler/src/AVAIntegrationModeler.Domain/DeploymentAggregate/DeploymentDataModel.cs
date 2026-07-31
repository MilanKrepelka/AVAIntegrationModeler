namespace AVAIntegrationModeler.Domain.DeploymentAggregate;

/// <summary>
/// Entita reprezentující datový model zahrnutý v nasazení.
/// </summary>
public class DeploymentDataModel : EntityBase<Guid>
{
  /// <summary>
  /// Privátní konstruktor pro EF Core.
  /// </summary>
  private DeploymentDataModel() { }

  /// <summary>
  /// Konstruktor pro vytvoření záznamu o datovém modelu v nasazení.
  /// </summary>
  /// <param name="deploymentId">Identifikátor nasazení.</param>
  /// <param name="dataModelId">Identifikátor datového modelu.</param>
  public DeploymentDataModel(Guid deploymentId, Guid dataModelId)
  {
    Id = Guid.NewGuid();
    DeploymentId = deploymentId;
    DataModelId = dataModelId;
  }

  /// <summary>
  /// Identifikátor nasazení.
  /// </summary>
  public Guid DeploymentId { get; private set; }

  /// <summary>
  /// Identifikátor datového modelu.
  /// </summary>
  public Guid DataModelId { get; private set; }
}
