namespace AVAIntegrationModeler.Contracts;

public class ExportDataModelRecordsRequest
{
  public Datasource Datasource { get; set; } = Datasource.Database;
  public List<Guid> RecordIds { get; set; } = new();
}
