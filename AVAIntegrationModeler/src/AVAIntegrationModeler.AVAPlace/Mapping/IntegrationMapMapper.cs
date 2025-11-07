using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using ASOL.DataService.Contracts;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.AVAPlace.Mapping;

/// <summary>
/// Statická třída pro mapování mezi DataModelDefinition z DataService a DataModelDTO/DataModelSummaryDTO.
/// </summary>
public static class IntegrationMapMapper
{
  /// <summary>
  /// Převede DataModelDefinition na DataModelDTO.
  /// </summary>
  /// <param name="integrationMapSummary">Definice datového modelu z DataService.</param>
  /// <returns>DataModelDTO.</returns>
  public static IntegrationMapSummaryDTO MapToSummaryDTO(IntegrationMapSummary integrationMapSummary)
  {
    Guard.Against.Null(integrationMapSummary, nameof(integrationMapSummary));

    return new IntegrationMapSummaryDTO
    {
      Id = Guid.Parse(integrationMapSummary.Id),
      Area = integrationMapSummary.AreaCode ?? string.Empty,
    };
  }

 

  /// <summary>
  /// Převede DataModelFieldDefinition na DataModelFieldDTO.
  /// </summary>
  /// <param name="fieldDefinition">Definice pole datového modelu.</param>
  /// <returns>DataModelFieldDTO.</returns>
  private static DataModelFieldDTO MapFieldToDTO(DataModelFieldDefinition fieldDefinition)
  {
    Guard.Against.Null(fieldDefinition, nameof(fieldDefinition));

    return new DataModelFieldDTO
    {
      // Id = fieldDefinition.Id,
      Name = fieldDefinition.Name ?? string.Empty,
      Label = fieldDefinition.Label ?? string.Empty,
      Description = fieldDefinition.Description ?? string.Empty,
      IsPublishedForLookup = fieldDefinition.IsPublishedForLookup,
      IsCollection = fieldDefinition.IsCollection,
      IsLocalized = fieldDefinition.IsLocalized,
      IsNullable = fieldDefinition.IsNullable,
      FieldType = MapFieldType(fieldDefinition.FieldType),
      ReferencedEntityTypeIds = fieldDefinition.ReferencedEntityTypeIds?.ToList() ?? new List<Guid>()
    };
  }

  /// <summary>
  /// Mapuje DataModelFieldType z DataService na aplikační DataModelFieldType.
  /// </summary>
  /// <param name="fieldType">Typ pole z DataService.</param>
  /// <returns>Aplikační DataModelFieldType.</returns>
  private static Contracts.DataModelFieldType MapFieldType(ASOL.DataService.Domain.Model.DataModelFieldType fieldType)
  {
    return fieldType switch
    {
      ASOL.DataService.Domain.Model.DataModelFieldType.Text => Contracts.DataModelFieldType.Text,
      ASOL.DataService.Domain.Model.DataModelFieldType.MultilineText => Contracts.DataModelFieldType.MultilineText,
      ASOL.DataService.Domain.Model.DataModelFieldType.TwoOptions => Contracts.DataModelFieldType.TwoOptions,
      ASOL.DataService.Domain.Model.DataModelFieldType.WholeNumber => Contracts.DataModelFieldType.WholeNumber,
      ASOL.DataService.Domain.Model.DataModelFieldType.DecimalNumber => Contracts.DataModelFieldType.DecimalNumber,
      ASOL.DataService.Domain.Model.DataModelFieldType.UniqueIdentifier => Contracts.DataModelFieldType.UniqueIdentifier,
      ASOL.DataService.Domain.Model.DataModelFieldType.UtcDateTime => Contracts.DataModelFieldType.UtcDateTime,
      ASOL.DataService.Domain.Model.DataModelFieldType.LookupEntity => Contracts.DataModelFieldType.LookupEntity,
      ASOL.DataService.Domain.Model.DataModelFieldType.NestedEntity => Contracts.DataModelFieldType.NestedEntity,
      ASOL.DataService.Domain.Model.DataModelFieldType.Date => Contracts.DataModelFieldType.Date,
      ASOL.DataService.Domain.Model.DataModelFieldType.FileReference => Contracts.DataModelFieldType.FileReference,
      ASOL.DataService.Domain.Model.DataModelFieldType.CurrencyNumber => Contracts.DataModelFieldType.CurrencyNumber,
      ASOL.DataService.Domain.Model.DataModelFieldType.SingleSelectOptionSet => Contracts.DataModelFieldType.SingleSelectOptionSet,
      ASOL.DataService.Domain.Model.DataModelFieldType.MultiSelectOptionSet => Contracts.DataModelFieldType.MultiSelectOptionSet,
      _ => Contracts.DataModelFieldType.Text // Default fallback
    };
  }
}
