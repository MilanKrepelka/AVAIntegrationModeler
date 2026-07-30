using AVAIntegrationModeler.Domain.AreaAggregate;

namespace AVAIntegrationModeler.Domain.AreaAggregate.Specifications;

/// <summary>
/// Specifikace pro vyhledání oblasti podle identifikátoru.
/// </summary>
public class AreaByIdSpec : Specification<Area>
{
  public AreaByIdSpec(Guid id) =>
    Query.Where(a => a.Id == id);
}
