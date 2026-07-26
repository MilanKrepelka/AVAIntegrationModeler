using AVAIntegrationModeler.Domain.DataModelAggregate;

namespace AVAIntegrationModeler.Domain.DataModelAggregate.Specifications;

public class DataModelByCodeSpec : Specification<DataModel>
{
  public DataModelByCodeSpec(string code) =>
    Query.Where(dataModel => dataModel.Code == code);
}
