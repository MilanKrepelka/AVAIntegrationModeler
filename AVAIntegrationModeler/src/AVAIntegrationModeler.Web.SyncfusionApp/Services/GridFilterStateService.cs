using Syncfusion.Blazor.Grids;

namespace AVAIntegrationModeler.Web.SyncfusionApp.Services;

/// <summary>Uchovává stav sloupcových filtrů SfGrid pro přehledové stránky v rámci jednoho Blazor circuit.</summary>
public class GridFilterStateService
{
  private readonly Dictionary<string, List<GridFilterColumn>> _state = new();

  /// <summary>Uloží predikáty filtru pro daný klíč stránky.</summary>
  public void Save(string key, IEnumerable<GridFilterColumn>? columns)
    => _state[key] = columns?.ToList() ?? [];

  /// <summary>Vrátí uložené predikáty filtru pro daný klíč, nebo prázdný seznam.</summary>
  public List<GridFilterColumn> GetOrEmpty(string key)
    => _state.TryGetValue(key, out var v) ? v : [];
}
