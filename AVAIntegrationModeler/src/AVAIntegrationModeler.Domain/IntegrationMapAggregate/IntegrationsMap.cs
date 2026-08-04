using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;

namespace AVAIntegrationModeler.Domain.IntegrationMapAggregate;

/// <summary>
/// Agregátní kořen pro integrační mapu - mapuje integrační scénáře k oblasti.
/// </summary>
public class IntegrationsMap : EntityBase<Guid>, IAggregateRoot
{
  /// <summary>
  /// Privátní kolekce integračních položek.
  /// </summary>
  private readonly List<IntegrationMapItem> _items = new();

  /// <summary>
  /// Konstruktor pro vytvoření nové integrační mapy.
  /// </summary>
  /// <param name="id">Identifikátor integrační mapy.</param>
  /// <param name="areaId">Identifikátor oblasti.</param>
  public IntegrationsMap(Guid id, Guid areaId)
  {
    Id = Guard.Against.Default(id, nameof(id));
    AreaId = Guard.Against.Default(areaId, nameof(areaId));
  }

  /// <summary>
  /// Privátní konstruktor pro EF Core.
  /// </summary>
  private IntegrationsMap()
  {
    // EF Core vyžaduje bezparametrový konstruktor
  }

  /// <summary>
  /// Identifikátor oblasti, ke které je mapa přiřazena.
  /// </summary>
  public Guid AreaId { get; private set; }

  /// <summary>
  /// Veřejný read-only přístup k integračním položkám.
  /// </summary>
  public IReadOnlyCollection<IntegrationMapItem> Items => _items.AsReadOnly();

  /// <summary>
  /// Přidá novou integrační položku do mapy.
  /// </summary>
  /// <param name="scenarioId">Identifikátor scénáře.</param>
  /// <returns>Nově vytvořená položka.</returns>
  public IntegrationMapItem AddItem(Guid scenarioId)
  {
    Guard.Against.Default(scenarioId, nameof(scenarioId));

    // Business rule: nelze přidat duplicitní scénář
    if (_items.Any(i => i.ScenarioId == scenarioId))
    {
      throw new InvalidOperationException($"Scenario {scenarioId} is already mapped in this integration map.");
    }

    var item = new IntegrationMapItem(Guid.NewGuid(), scenarioId);
    _items.Add(item);
    
    return item;
  }

  /// <summary>
  /// Odebere integrační položku podle ID scénáře.
  /// </summary>
  /// <param name="scenarioId">Identifikátor scénáře k odebrání.</param>
  /// <returns>True, pokud byla položka odebrána, jinak false.</returns>
  public bool RemoveItem(Guid scenarioId)
  {
    var item = _items.FirstOrDefault(i => i.ScenarioId == scenarioId);
    if (item == null)
    {
      return false;
    }

    return _items.Remove(item);
  }

  /// <summary>
  /// Získá integrační položku podle ID scénáře.
  /// </summary>
  /// <param name="scenarioId">Identifikátor scénáře.</param>
  /// <returns>Položka nebo null, pokud není nalezena.</returns>
  public IntegrationMapItem? GetItem(Guid scenarioId)
  {
    return _items.FirstOrDefault(i => i.ScenarioId == scenarioId);
  }

  /// <summary>
  /// Změní oblast přiřazenou k této mapě.
  /// </summary>
  /// <param name="newAreaId">Nový identifikátor oblasti.</param>
  /// <returns>Aktuální instance pro fluent API.</returns>
  public IntegrationsMap SetArea(Guid newAreaId)
  {
    AreaId = Guard.Against.Default(newAreaId, nameof(newAreaId));
    return this;
  }
}
