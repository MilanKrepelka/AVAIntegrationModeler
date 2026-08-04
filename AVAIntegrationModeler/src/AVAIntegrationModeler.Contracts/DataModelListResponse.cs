using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Contracts;

/// <summary>
/// Odpověď na požadavek <see cref="DataModelListRequest"/>
/// </summary>
public class DataModelListResponse
{
  /// <summary>
  /// Seznam <see cref="DataModelDTO"/>
  /// </summary>
  public List<DataModelDTO> DataModels { get; set; } = [];
}
