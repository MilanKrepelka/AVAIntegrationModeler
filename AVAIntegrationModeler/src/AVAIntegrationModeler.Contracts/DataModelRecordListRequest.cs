namespace AVAIntegrationModeler.Contracts;

public class DataModelRecordListRequest
{
  public Datasource Datasource { get; set; } = Datasource.Database;
  public Guid? ModelId { get; set; }
}
