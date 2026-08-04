namespace AVAIntegrationModeler.Contracts;

public class DeleteDataModelRecordRequest
{
  public const string Route = "/DataModelRecords/{Datasource}/{RecordId:guid}";

  public static string BuildRoute(Guid recordId, Datasource datasource = Datasource.Database) =>
    Route.Replace("{Datasource}", datasource.ToString())
         .Replace("{RecordId:guid}", recordId.ToString());

  public Guid RecordId { get; set; }
  public Datasource Datasource { get; set; } = Datasource.Database;
}
