using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Contracts;

public class CreateDataModelRecordRequest
{
  public const string Route = "/DataModelRecords";
  public Datasource Datasource { get; set; } = Datasource.Database;
  public DataModelRecordDTO Record { get; set; } = new DataModelRecordDTO();
}
