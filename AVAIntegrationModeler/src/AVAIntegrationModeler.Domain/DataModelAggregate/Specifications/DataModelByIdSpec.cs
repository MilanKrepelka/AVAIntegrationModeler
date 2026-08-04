using AVAIntegrationModeler.Domain.DataModelAggregate;

namespace AVAIntegrationModeler.Domain.DataModelAggregate.Specifications;

public class DataModelByIdSpec : Specification<DataModel>
{
  public DataModelByIdSpec(Guid dataModelId) =>
    Query
        .Where(dataModel => dataModel.Id == dataModelId);
}
