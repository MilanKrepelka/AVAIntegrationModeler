using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.Contracts;

public class UpdateDataModelResponse
{
  public UpdateDataModelResponse(DataModelDTO dataModel) => DataModel = dataModel;
  public DataModelDTO DataModel { get; set; }
}
