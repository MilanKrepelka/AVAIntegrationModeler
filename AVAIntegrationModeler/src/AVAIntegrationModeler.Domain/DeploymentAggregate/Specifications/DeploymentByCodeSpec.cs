namespace AVAIntegrationModeler.Domain.DeploymentAggregate.Specifications;

/// <summary>
/// Specifikace pro vyhledání nasazení podle kódu. Porovnání kódu je case-insensitive.
/// </summary>
public class DeploymentByCodeSpec : Specification<Deployment>
{
  public DeploymentByCodeSpec(string code) =>
    Query.Where(d => d.Code.ToUpper() == code.ToUpper()).Include(d => d.DataModels);
}
