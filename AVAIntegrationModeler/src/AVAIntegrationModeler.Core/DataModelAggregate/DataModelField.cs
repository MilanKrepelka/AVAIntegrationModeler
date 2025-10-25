using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Core.DataModelAggregate;

/// <summary>
/// Třída představující pole datového modelu. V klasickém pojetí tříd je to atribut nebo property.
/// </summary>
public class DataModelField
{
  /// <summary>
  /// Název pole datového modelu.
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// Lidsky čitelný popisek pole datového modelu.
  /// </summary>
  public string Label { get; set; }= string.Empty;

  /// <summary>
  /// Popis pole datového modelu.
  /// </summary>
  public string Description { get; set; } = string.Empty;

  /// <summary>
  /// Příznak určující, zda je toto pole publikováno pro vyhledávání (lookup).
  /// </summary>
  public bool IsPublishedForLookup { get; set; }

  /// <summary>
  /// Příznak určující, zda je toto pole kolekcí (více hodnot).
  /// </summary>
  public bool IsCollection { get; set; }

  /// <summary>
  /// Příznak určující, zda je toto pole lokalizované.
  /// </summary>
  public bool IsLocalized { get; set; }

  /// <summary>
  /// Příznak určující, zda může být toto pole null (prázdné).
  /// </summary>
  public bool IsNullable { get; set; }

  /// <summary>
  /// Reprezentuje typ pole datového modelu.<see cref="DataModelFieldType"/>
  /// </summary>
  public DataModelFieldType FieldType { get; set; }

  /// <summary>
  /// Identifikátory typů entit, na které toto pole odkazuje (pouze pro typy LookupEntity a NestedEntity).
  /// </summary>
  public List<Guid> ReferencedEntityTypeIds { get; set; } = new List<Guid>();
}
