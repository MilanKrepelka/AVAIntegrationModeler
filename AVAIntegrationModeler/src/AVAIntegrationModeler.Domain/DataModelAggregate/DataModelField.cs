using System;
using System.Collections.Generic;
using System.Linq;
using AVAIntegrationModeler.Contracts;

namespace AVAIntegrationModeler.Domain.DataModelAggregate;

/// <summary>
/// Entita představující pole datového modelu. V klasickém pojetí tříd je to atribut nebo property.
/// </summary>
public class DataModelField : EntityBase<Guid>
{
  // Privátní konstruktor pro EF Domain
  private DataModelField() { }

  /// <summary>
  /// Konstruktor pro vytvoření nového pole datového modelu.
  /// </summary>
  /// <param name="id">Identifikátor pole.</param>
  /// <param name="name">Název pole.</param>
  /// <param name="fieldType">Typ pole.</param>
  public DataModelField(Guid id, string name, DataModelFieldType fieldType)
  {
    Id = id;
    SetName(name);
    SetFieldType(fieldType);
  }

  /// <summary>
  /// Název pole datového modelu.
  /// </summary>
  public string Name { get; private set; } = string.Empty;

  /// <summary>
  /// Lidsky čitelný popisek pole datového modelu.
  /// </summary>
  public string Label { get; private set; } = string.Empty;

  /// <summary>
  /// Popis pole datového modelu.
  /// </summary>
  public string Description { get; private set; } = string.Empty;

  /// <summary>
  /// Příznak určující, zda je toto pole publikováno pro vyhledávání (lookup).
  /// </summary>
  public bool IsPublishedForLookup { get; private set; }

  /// <summary>
  /// Příznak určující, zda je toto pole kolekcí (více hodnot).
  /// </summary>
  public bool IsCollection { get; private set; }

  /// <summary>
  /// Příznak určující, zda je toto pole lokalizované.
  /// </summary>
  public bool IsLocalized { get; private set; }

  /// <summary>
  /// Příznak určující, zda může být toto pole null (prázdné).
  /// </summary>
  public bool IsNullable { get; private set; }

  /// <summary>
  /// Reprezentuje typ pole datového modelu. <see cref="DataModelFieldType"/>
  /// </summary>
  public DataModelFieldType FieldType { get; private set; }

  /// <summary>
  /// Privátní kolekce odkazů na typy entit.
  /// </summary>
  private readonly List<DataModelFieldEntityTypeReference> _entityTypeReferences = new();

  /// <summary>
  /// Read-only kolekce odkazů na typy entit.
  /// </summary>
  public IReadOnlyCollection<DataModelFieldEntityTypeReference> EntityTypeReferences 
    => _entityTypeReferences.AsReadOnly();

  /// <summary>
  /// Helper property pro získání pouze ID odkazovaných typů (pro zpětnou kompatibilitu).
  /// </summary>
  public IEnumerable<Guid> ReferencedEntityTypeIds 
    => _entityTypeReferences.Select(r => r.ReferencedEntityTypeId);

  /// <summary>
  /// Nastaví název pole.
  /// </summary>
  /// <param name="name">Nový název pole.</param>
  public DataModelField SetName(string name)
  {
    Name = Guard.Against.NullOrEmpty(name, nameof(name));
    return this;
  }

  /// <summary>
  /// Nastaví popisek pole.
  /// </summary>
  /// <param name="label">Nový popisek pole.</param>
  public DataModelField SetLabel(string label)
  {
    Label = Guard.Against.NullOrEmpty(label, nameof(label));
    return this;
  }

  /// <summary>
  /// Nastaví popis pole.
  /// </summary>
  /// <param name="description">Nový popis pole.</param>
  public DataModelField SetDescription(string description)
  {
    Description = description ?? string.Empty;
    return this;
  }

  /// <summary>
  /// Nastaví typ pole.
  /// </summary>
  /// <param name="fieldType">Nový typ pole.</param>
  public DataModelField SetFieldType(DataModelFieldType fieldType)
  {
    FieldType = Guard.Against.EnumOutOfRange(fieldType, nameof(fieldType));
    return this;
  }

  /// <summary>
  /// Označí pole jako publikované pro vyhledávání.
  /// </summary>
  public DataModelField MarkAsPublishedForLookup()
  {
    IsPublishedForLookup = true;
    return this;
  }

  /// <summary>
  /// Označí pole jako kolekci.
  /// </summary>
  public DataModelField MarkAsCollection()
  {
    IsCollection = true;
    return this;
  }

  /// <summary>
  /// Označí pole jako lokalizované.
  /// </summary>
  public DataModelField MarkAsLocalized()
  {
    IsLocalized = true;
    return this;
  }

  /// <summary>
  /// Označí pole jako nullable.
  /// </summary>
  public DataModelField MarkAsNullable()
  {
    IsNullable = true;
    return this;
  }

  /// <summary>
  /// Přidá odkaz na typ entity (pro LookupEntity nebo NestedEntity).
  /// </summary>
  public DataModelField AddReferencedEntityType(Guid entityTypeId)
  {
    Guard.Against.Default(entityTypeId, nameof(entityTypeId));

    if (FieldType != DataModelFieldType.LookupEntity && FieldType != DataModelFieldType.NestedEntity)
    {
      throw new InvalidOperationException(
        $"Referenced entity types can only be added for FieldType LookupEntity or NestedEntity. Current type: {FieldType}");
    }

    // Kontrola duplicit
    if (_entityTypeReferences.Any(r => r.ReferencedEntityTypeId == entityTypeId))
    {
      return this; // již existuje
    }

    var reference = new DataModelFieldEntityTypeReference(
      Guid.NewGuid(), 
      this.Id, 
      entityTypeId
    );
    
    _entityTypeReferences.Add(reference);
    return this;
  }

  /// <summary>
  /// Odebere odkaz na typ entity.
  /// </summary>
  public DataModelField RemoveReferencedEntityType(Guid entityTypeId)
  {
    var reference = _entityTypeReferences
      .FirstOrDefault(r => r.ReferencedEntityTypeId == entityTypeId);
    
    if (reference != null)
    {
      _entityTypeReferences.Remove(reference);
    }
    
    return this;
  }
}
