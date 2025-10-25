using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVAIntegrationModeler.Core.DataModelAggregate;
public class DataModel : EntityBase<Guid>, IAggregateRoot
{
  /// <summary>
  /// Kód datového modelu, který slouží k jeho jednoznačné identifikaci v rámci systému.
  /// </summary>
  public string Code { get; set; } = string.Empty;

  /// <summary>
  /// Název datového modelu.
  /// </summary>
  public string Name { get; set; } = string.Empty;

  /// <summary>
  /// Popis datového modelu.
  /// </summary>
  public string Description { get; set; } = string.Empty;

  /// <summary>
  /// Poznámky k datovému modelu.
  /// </summary>
  public string Notes { get; set; } = string.Empty;

  /// <summary>
  /// Příznak určující, zda je tento datový model kořenem agregátu. V opačném případě je to Nested entity
  /// </summary>
  public bool IsAggregateRoot { get; set; } = false;

  /// <summary>
  /// Seznam  Fieldů/Atributů, která tvoří strukturu datového modelu.
  /// </summary>
  public List<DataModelField> Fields { get; set; } = new List<DataModelField>();

}
