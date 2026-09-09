namespace AVAIntegrationModeler.Contracts.DataModels;

/// <summary>
/// Požadavek na porovnání datového modelu mezi databází a AVAPlace.
/// </summary>
public class GetDataModelChangesRequest
{
  /// <summary>Identifikátor datového modelu.</summary>
  public Guid DataModelId { get; set; }
}
