using System;
using System.Collections.Generic;

namespace AVAIntegrationModeler.Contracts.DTO;

/// <summary>
/// DTO pro datový model.
/// </summary>
public record DataModelDTO
{
  /// <summary>
  /// Jedinečný identifikátor datového modelu.
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Kód datového modelu, který slouží k jeho jednoznačné identifikaci v rámci systému.
  /// </summary>
  public string Code { get; init; } = string.Empty;

  /// <summary>
  /// Název datového modelu.
  /// </summary>
  public string Name { get; init; } = string.Empty;

  /// <summary>
  /// Popis datového modelu.
  /// </summary>
  public string Description { get; init; } = string.Empty;

  /// <summary>
  /// Poznámky k datovému modelu.
  /// </summary>
  public string Notes { get; init; } = string.Empty;

  /// <summary>
  /// Příznak určující, zda je tento datový model kořenem agregátu.
  /// </summary>
  public bool IsAggregateRoot { get; init; }

  /// <summary>
  /// Identifikátor oblasti (Area), ke které datový model patří.
  /// </summary>
  public Guid? AreaId { get; init; }

  /// <summary>
  /// Seznam fieldů/atributů datového modelu.
  /// </summary>
  public List<DataModelFieldDTO> Fields { get; init; } = new();
}
