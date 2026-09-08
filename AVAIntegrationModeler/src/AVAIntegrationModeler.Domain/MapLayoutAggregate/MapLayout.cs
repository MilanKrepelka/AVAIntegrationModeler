namespace AVAIntegrationModeler.Domain.MapLayoutAggregate;

/// <summary>
/// Ukládá JSON diagramu pro pojmenované mapy (např. mapa všech oblastí VDM).
/// </summary>
public class MapLayout : DomainEntityBase<Guid>, IAggregateRoot
{
  public MapLayout() { } // EF Core

  public MapLayout(Guid id, string key)
  {
    Id = id;
    SetKey(key);
  }

  public string Key { get; private set; } = string.Empty;
  public string? DiagramJson { get; private set; }
  public DateTime? LastMapSave { get; private set; }

  public MapLayout SetKey(string key)
  {
    Key = Guard.Against.NullOrEmpty(key, nameof(key));
    return this;
  }

  public MapLayout SetDiagram(string diagramJson)
  {
    DiagramJson = Guard.Against.NullOrEmpty(diagramJson, nameof(diagramJson));
    LastMapSave = DateTime.UtcNow;
    return this;
  }
}
