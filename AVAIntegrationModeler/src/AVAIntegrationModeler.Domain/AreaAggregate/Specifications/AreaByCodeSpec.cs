using AVAIntegrationModeler.Domain.AreaAggregate;

namespace AVAIntegrationModeler.Domain.AreaAggregate.Specifications;

/// <summary>
/// Specifikace pro vyhledání oblasti podle kódu.
/// </summary>
public class AreaByCodeSpec : Specification<Area>
{
  public AreaByCodeSpec(string code) =>
    Query.Where(a => a.Code == code);
}
