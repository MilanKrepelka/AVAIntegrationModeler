using AVAIntegrationModeler.Core.ScenarioAggregate;

namespace AVAIntegrationModeler.Core.ScenarioAggregate.Specifications;

public class FeatureByIdSpec : Specification<Feature>
{
  public FeatureByIdSpec(Guid featureId) =>
    Query
        .Where(feature => feature.Id == featureId);
}
