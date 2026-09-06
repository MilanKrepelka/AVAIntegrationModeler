namespace AVAIntegrationModeler.Domain.MapLayoutAggregate.Specs;

/// <summary>
/// Specifikace pro načtení MapLayout podle klíče.
/// </summary>
public class MapLayoutByKeySpec : Specification<MapLayout>
{
  public MapLayoutByKeySpec(string key) =>
    Query.Where(m => m.Key == key);
}
