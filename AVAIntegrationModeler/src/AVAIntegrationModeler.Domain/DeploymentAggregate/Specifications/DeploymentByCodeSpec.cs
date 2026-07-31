namespace AVAIntegrationModeler.Domain.DeploymentAggregate.Specifications;

/// <summary>
/// Specifikace pro vyhledání nasazení podle kódu.
/// </summary>
public class DeploymentByCodeSpec : Specification<Deployment>
{
  public DeploymentByCodeSpec(string code) =>
    Query.Where(d => d.Code == code).Include(d => d.DataModels);
}
