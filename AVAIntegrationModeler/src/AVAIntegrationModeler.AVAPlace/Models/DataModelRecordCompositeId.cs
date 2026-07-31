namespace AVAIntegrationModeler.AVAPlace.Models;

/// <summary>
/// Složený identifikátor záznamu datového modelu z AVAPlace DataService.
/// </summary>
public sealed class DataModelRecordCompositeId
{
  /// <summary>
  /// Identifikátor datového modelu.
  /// </summary>
  public Guid ModelId { get; set; }

  /// <summary>
  /// Identifikátor záznamu.
  /// </summary>
  public Guid RecordId { get; set; }
}
