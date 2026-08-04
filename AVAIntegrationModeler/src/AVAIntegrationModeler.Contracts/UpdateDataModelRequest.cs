using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Contracts;

public class UpdateDataModelRequest
{
  public const string Route = "/DataModels/{Datasource}/{DataModelId:guid}";

  public static string BuildRoute(Guid dataModelId, Datasource datasource = Datasource.Database) =>
    Route.Replace("{Datasource}", datasource.ToString())
         .Replace("{DataModelId:guid}", dataModelId.ToString());

  public Guid DataModelId { get; set; }
  public Datasource Datasource { get; set; } = Datasource.Database;
  public DataModelDTO DataModel { get; set; } = new DataModelDTO();
}
