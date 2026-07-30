namespace AVAIntegrationModeler.Contracts;

public class ImportDataModelFromAvaPlaceRequest
{
  public const string Route = "/DataModels/ImportFromAVAPlace";

  public Guid AvaPlaceModelId { get; set; }
}
