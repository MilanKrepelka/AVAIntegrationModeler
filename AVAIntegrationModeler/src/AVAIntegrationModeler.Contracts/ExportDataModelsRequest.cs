namespace AVAIntegrationModeler.Contracts;

public class ExportDataModelsRequest
{
  public Datasource Datasource { get; set; } = Datasource.Database;
  public List<Guid> ModelIds { get; set; } = new();
}
