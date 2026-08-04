using System;
using System.Collections.Generic;
using AVAIntegrationModeler.Contracts; 

namespace AVAIntegrationModeler.Contracts.DTO;

/// <summary>
/// DTO pro pole datového modelu.
/// </summary>
public record DataModelFieldDTO
{
  /// <summary>
  /// Jedinečný identifikátor pole.
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Název pole datového modelu.
  /// </summary>
  public string Name { get; init; } = string.Empty;

  /// <summary>
  /// Lidsky čitelný popisek pole datového modelu.
  /// </summary>
  public string Label { get; init; } = string.Empty;

  /// <summary>
  /// Popis pole datového modelu.
  /// </summary>
  public string Description { get; init; } = string.Empty;

  /// <summary>
  /// Příznak určující, zda je toto pole publikováno pro vyhledávání (lookup).
  /// </summary>
  public bool IsPublishedForLookup { get; init; }

  /// <summary>
  /// Příznak určující, zda je toto pole kolekcí (více hodnot).
  /// </summary>
  public bool IsCollection { get; init; }

  /// <summary>
  /// Příznak určující, zda je toto pole lokalizované.
  /// </summary>
  public bool IsLocalized { get; init; }

  /// <summary>
  /// Příznak určující, zda může být toto pole null (prázdné).
  /// </summary>
  public bool IsNullable { get; init; }

  /// <summary>
  /// Reprezentuje typ pole datového modelu.
  /// </summary>
  public DataModelFieldType FieldType { get; init; } 

  /// <summary>
  /// Identifikátory typů entit, na které toto pole odkazuje (pouze pro typy LookupEntity a NestedEntity).
  /// </summary>
  public List<Guid> ReferencedEntityTypeIds { get; init; } = new();
}
