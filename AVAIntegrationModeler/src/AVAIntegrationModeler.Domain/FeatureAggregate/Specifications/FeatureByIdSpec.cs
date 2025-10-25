using AVAIntegrationModeler.Domain.ScenarioAggregate;

namespace AVAIntegrationModeler.Domain.ScenarioAggregate.Specifications;

public class FeatureByIdSpec : Specification<Guid>
{
  public FeatureByIdSpec(Guid featureId) =>
    Query
        .Where( feature => feature == featureId);
}
