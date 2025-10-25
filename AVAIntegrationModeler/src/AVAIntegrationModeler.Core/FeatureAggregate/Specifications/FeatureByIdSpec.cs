using AVAIntegrationModeler.Core.ScenarioAggregate;

namespace AVAIntegrationModeler.Core.ScenarioAggregate.Specifications;

public class FeatureByIdSpec : Specification<Guid>
{
  public FeatureByIdSpec(Guid featureId) =>
    Query
        .Where( feature => feature == featureId);
}
