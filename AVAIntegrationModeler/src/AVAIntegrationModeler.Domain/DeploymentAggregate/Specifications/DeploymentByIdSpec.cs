namespace AVAIntegrationModeler.Domain.DeploymentAggregate.Specifications;

/// <summary>
/// Specifikace pro vyhledání nasazení podle identifikátoru.
/// </summary>
public class DeploymentByIdSpec : Specification<Deployment>
{
  public DeploymentByIdSpec(Guid id) =>
    Query.Where(d => d.Id == id).Include(d => d.DataModels);
}
