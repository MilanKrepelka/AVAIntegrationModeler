using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Contracts;

public class DataModelRecordListResponse
{
  public List<DataModelRecordDTO> Records { get; set; } = new();
}
