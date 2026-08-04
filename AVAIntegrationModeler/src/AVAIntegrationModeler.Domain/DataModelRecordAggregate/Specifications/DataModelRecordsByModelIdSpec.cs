namespace AVAIntegrationModeler.Domain.DataModelRecordAggregate.Specifications;

public class DataModelRecordsByModelIdSpec : Specification<DataModelRecord>
{
  public DataModelRecordsByModelIdSpec(Guid modelId)
  {
    Query.Where(r => r.ModelId == modelId);
  }
}
