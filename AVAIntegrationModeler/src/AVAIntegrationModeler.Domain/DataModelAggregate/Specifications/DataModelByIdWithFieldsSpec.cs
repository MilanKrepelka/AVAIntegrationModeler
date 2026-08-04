using AVAIntegrationModeler.Domain.DataModelAggregate;

namespace AVAIntegrationModeler.Domain.DataModelAggregate.Specifications;

public class DataModelByIdWithFieldsSpec : Specification<DataModel>
{
  public DataModelByIdWithFieldsSpec(Guid id) =>
    Query.Where(dm => dm.Id == id);
}
