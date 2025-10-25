using System;

namespace AVAIntegrationModeler.Domain.DataModelAggregate;

/// <summary>
/// Reprezentuje odkaz na typ entity z DataModelField (pro LookupEntity/NestedEntity).
/// Jedná se o join/mapping tabulku mezi DataModelField a odkazovanými typy entit.
/// </summary>
public class DataModelFieldEntityTypeReference : EntityBase<Guid>
{
  // Privátní konstruktor pro EF Core
  private DataModelFieldEntityTypeReference() { }

  /// <summary>
  /// Konstruktor pro vytvoření nového odkazu.
  /// </summary>
  /// <param name="id">Identifikátor odkazu.</param>
  /// <param name="dataModelFieldId">ID pole datového modelu.</param>
  /// <param name="referencedEntityTypeId">ID odkazované entity.</param>
  public DataModelFieldEntityTypeReference(Guid id, Guid dataModelFieldId, Guid referencedEntityTypeId)
  {
    Id = id;
    DataModelFieldId = Guard.Against.Default(dataModelFieldId, nameof(dataModelFieldId));
    ReferencedEntityTypeId = Guard.Against.Default(referencedEntityTypeId, nameof(referencedEntityTypeId));
  }

  /// <summary>
  /// ID DataModelField, ke kterému tento odkaz patří.
  /// </summary>
  public Guid DataModelFieldId { get; private set; }

  /// <summary>
  /// ID odkazovaného typu entity (např. ID jiného DataModel).
  /// </summary>
  public Guid ReferencedEntityTypeId { get; private set; }
}
