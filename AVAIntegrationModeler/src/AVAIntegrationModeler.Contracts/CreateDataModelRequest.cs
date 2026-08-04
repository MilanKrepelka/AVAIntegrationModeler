using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Contracts;

public class CreateDataModelRequest
{
  public const string Route = "/DataModels";
  public Datasource Datasource { get; set; } = Datasource.Database;
  public DataModelDTO DataModel { get; set; } = new DataModelDTO();
}
