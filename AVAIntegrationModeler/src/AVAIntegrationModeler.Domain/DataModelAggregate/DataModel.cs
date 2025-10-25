  using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Domain.DataModelAggregate;

/// <summary>
/// Aggregate root pro datový model.
/// </summary>
public class DataModel : EntityBase<Guid>, IAggregateRoot
{
  // Privátní konstruktor pro EF Domain
  private DataModel() { }

  /// <summary>
  /// Konstruktor pro vytvoření nového datového modelu.
  /// </summary>
  /// <param name="id">Identifikátor datového modelu.</param>
  /// <param name="code">Kód datového modelu.</param>
  public DataModel(Guid id, string code)
  {
    Id = id;
    SetCode(code);
  }

  /// <summary>
  /// Kód datového modelu, který slouží k jeho jednoznačné identifikaci v rámci systému.
  /// </summary>
  public string Code { get; private set; } = string.Empty;

  /// <summary>
  /// Název datového modelu.
  /// </summary>
  public string Name { get; private set; } = string.Empty;

  /// <summary>
  /// Popis datového modelu.
  /// </summary>
  public string Description { get; private set; } = string.Empty;

  /// <summary>
  /// Poznámky k datovému modelu.
  /// </summary>
  public string Notes { get; private set; } = string.Empty;

  /// <summary>
  /// Příznak určující, zda je tento datový model kořenem agregátu. V opačném případě je to Nested entity.
  /// </summary>
  public bool IsAggregateRoot { get; private set; } = false;

  /// <summary>
  /// Identifikátor oblasti (Area), ke které datový model patří.
  /// </summary>
  public Guid? AreaId { get; private set; }

  /// <summary>
  /// Privátní kolekce fieldů/atributů, která tvoří strukturu datového modelu.
  /// </summary>
  private readonly List<DataModelField> _fields = new();

  /// <summary>
  /// Read-only kolekce fieldů datového modelu.
  /// </summary>
  public IReadOnlyCollection<DataModelField> Fields => _fields.AsReadOnly();

  /// <summary>
  /// Nastaví kód datového modelu.
  /// </summary>
  /// <param name="code">Nový kód.</param>
  public DataModel SetCode(string code)
  {
    Code = Guard.Against.NullOrEmpty(code, nameof(code));
    return this;
  }

  /// <summary>
  /// Nastaví název datového modelu.
  /// </summary>
  /// <param name="name">Nový název.</param>
  public DataModel SetName(string name)
  {
    Name = Guard.Against.NullOrEmpty(name, nameof(name));
    return this;
  }

  /// <summary>
  /// Nastaví popis datového modelu.
  /// </summary>
  /// <param name="description">Nový popis.</param>
  public DataModel SetDescription(string description)
  {
    Description = description ?? string.Empty;
    return this;
  }

  /// <summary>
  /// Nastaví poznámky k datovému modelu.
  /// </summary>
  /// <param name="notes">Nové poznámky.</param>
  public DataModel SetNotes(string notes)
  {
    Notes = notes ?? string.Empty;
    return this;
  }

  /// <summary>
  /// Označí datový model jako Aggregate Root.
  /// </summary>
  public DataModel MarkAsAggregateRoot()
  {
    IsAggregateRoot = true;
    return this;
  }

  /// <summary>
  /// Označí datový model jako Nested Entity (ne Aggregate Root).
  /// </summary>
  public DataModel MarkAsNestedEntity()
  {
    IsAggregateRoot = false;
    return this;
  }

  /// <summary>
  /// Nastaví identifikátor oblasti (Area).
  /// </summary>
  /// <param name="areaId">Identifikátor oblasti.</param>
  public DataModel SetArea(Guid? areaId)
  {
    AreaId = areaId;
    return this;
  }

  /// <summary>
  /// Přidá field do datového modelu.
  /// </summary>
  /// <param name="field">Field k přidání.</param>
  public DataModel AddField(DataModelField field)
  {
    Guard.Against.Null(field, nameof(field));

    // Business rule: nelze mít duplicate field names
    if (_fields.Any(f => f.Name.Equals(field.Name, StringComparison.OrdinalIgnoreCase)))
    {
      throw new InvalidOperationException($"Field with name '{field.Name}' already exists in this data model.");
    }

    _fields.Add(field);
    return this;
  }

  /// <summary>
  /// Odebere field z datového modelu podle názvu.
  /// </summary>
  /// <param name="fieldName">Název fieldu k odebrání.</param>
  public DataModel RemoveField(string fieldName)
  {
    Guard.Against.NullOrEmpty(fieldName, nameof(fieldName));

    var field = _fields.FirstOrDefault(f => f.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
    if (field != null)
    {
      _fields.Remove(field);
    }

    return this;
  }

  /// <summary>
  /// Najde field podle názvu.
  /// </summary>
  /// <param name="fieldName">Název fieldu.</param>
  /// <returns>Field nebo null pokud neexistuje.</returns>
  public DataModelField? GetField(string fieldName)
  {
    Guard.Against.NullOrEmpty(fieldName, nameof(fieldName));
    return _fields.FirstOrDefault(f => f.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
  }
}
