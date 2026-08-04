namespace AVAIntegrationModeler.Domain.DataModelRecordAggregate.Specifications;

public class DataModelRecordByIdSpec : Specification<DataModelRecord>
{
  public DataModelRecordByIdSpec(Guid id)
  {
    Query.Where(r => r.Id == id);
  }
}
