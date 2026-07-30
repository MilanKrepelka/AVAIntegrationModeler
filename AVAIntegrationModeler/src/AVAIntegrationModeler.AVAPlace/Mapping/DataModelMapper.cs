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
public static class DataModelMapper
{
  /// <summary>
  /// Převede DataModelDefinition na DataModelDTO.
  /// </summary>
  /// <param name="dataModelDefinition">Definice datového modelu z DataService.</param>
  /// <returns>DataModelDTO.</returns>
  public static DataModelDTO MapToDTO(DataModelDefinition dataModelDefinition)
  {
    Guard.Against.Null(dataModelDefinition, nameof(dataModelDefinition));

    return new DataModelDTO
    {
      Id = dataModelDefinition.Id,
      Code = dataModelDefinition.Code ?? string.Empty,
      Name = dataModelDefinition.Name ?? string.Empty,
      Description = dataModelDefinition.Description ?? string.Empty,
      Notes = dataModelDefinition.Notes ?? string.Empty,
      IsAggregateRoot = dataModelDefinition.IsAggregateRoot,
      //AreaId = dataModelDefinition.AreaId,
      Fields = dataModelDefinition.Fields?.Select(MapFieldToDTO).ToList() ?? new List<DataModelFieldDTO>()
    };
  }

  /// <summary>
  /// Převede DataModelSummaryDTO na DataModelSummaryDTO.
  /// </summary>
  /// <param name="dataModelDefinition">Definice datového modelu z DataService.</param>
  /// <returns>DataModelSummaryDTO.</returns>
  public static DataModelSummaryDTO MapToSummaryDTO(DataModelDefinition dataModelDefinition)
  {
    Guard.Against.Null(dataModelDefinition, nameof(dataModelDefinition));

    return new DataModelSummaryDTO
    {
      Id = dataModelDefinition.Id,
      Code = dataModelDefinition.Code ?? string.Empty,
      Name = dataModelDefinition.Name ?? string.Empty
    };
  }

  /// <summary>
  /// Převede DataModelDTO na DataModelDefinition pro export do ASOL DataService.
  /// Pole jsou seřazena abecedně dle Name.
  /// </summary>
  /// <param name="dto">DTO datového modelu.</param>
  /// <returns>DataModelDefinition.</returns>
  public static DataModelDefinition MapToDefinition(DataModelDTO dto)
  {
    Guard.Against.Null(dto, nameof(dto));

    return new DataModelDefinition
    {
      Id = dto.Id,
      Code = dto.Code,
      Name = dto.Name,
      Description = dto.Description,
      Notes = dto.Notes,
      IsAggregateRoot = dto.IsAggregateRoot,
      Fields = dto.Fields
        .OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase)
        .Select(MapFieldToDefinition)
        .ToList()
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
  /// Převede DataModelFieldDTO na DataModelFieldDefinition.
  /// </summary>
  /// <param name="fieldDto">DTO pole datového modelu.</param>
  /// <returns>DataModelFieldDefinition.</returns>
  private static DataModelFieldDefinition MapFieldToDefinition(DataModelFieldDTO fieldDto)
  {
    Guard.Against.Null(fieldDto, nameof(fieldDto));

    return new DataModelFieldDefinition
    {
      Name = fieldDto.Name,
      Label = fieldDto.Label,
      Description = fieldDto.Description,
      IsPublishedForLookup = fieldDto.IsPublishedForLookup,
      IsCollection = fieldDto.IsCollection,
      IsLocalized = fieldDto.IsLocalized,
      IsNullable = fieldDto.IsNullable,
      FieldType = MapFieldTypeToAsol(fieldDto.FieldType),
      ReferencedEntityTypeIds = fieldDto.ReferencedEntityTypeIds?.ToList() ?? new List<Guid>()
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
      _ => Contracts.DataModelFieldType.Text
    };
  }

  /// <summary>
  /// Mapuje aplikační DataModelFieldType na DataModelFieldType z DataService.
  /// </summary>
  /// <param name="fieldType">Aplikační typ pole.</param>
  /// <returns>DataService DataModelFieldType.</returns>
  private static ASOL.DataService.Domain.Model.DataModelFieldType MapFieldTypeToAsol(Contracts.DataModelFieldType fieldType)
  {
    return fieldType switch
    {
      Contracts.DataModelFieldType.Text => ASOL.DataService.Domain.Model.DataModelFieldType.Text,
      Contracts.DataModelFieldType.MultilineText => ASOL.DataService.Domain.Model.DataModelFieldType.MultilineText,
      Contracts.DataModelFieldType.TwoOptions => ASOL.DataService.Domain.Model.DataModelFieldType.TwoOptions,
      Contracts.DataModelFieldType.WholeNumber => ASOL.DataService.Domain.Model.DataModelFieldType.WholeNumber,
      Contracts.DataModelFieldType.DecimalNumber => ASOL.DataService.Domain.Model.DataModelFieldType.DecimalNumber,
      Contracts.DataModelFieldType.UniqueIdentifier => ASOL.DataService.Domain.Model.DataModelFieldType.UniqueIdentifier,
      Contracts.DataModelFieldType.UtcDateTime => ASOL.DataService.Domain.Model.DataModelFieldType.UtcDateTime,
      Contracts.DataModelFieldType.LookupEntity => ASOL.DataService.Domain.Model.DataModelFieldType.LookupEntity,
      Contracts.DataModelFieldType.NestedEntity => ASOL.DataService.Domain.Model.DataModelFieldType.NestedEntity,
      Contracts.DataModelFieldType.Date => ASOL.DataService.Domain.Model.DataModelFieldType.Date,
      Contracts.DataModelFieldType.FileReference => ASOL.DataService.Domain.Model.DataModelFieldType.FileReference,
      Contracts.DataModelFieldType.CurrencyNumber => ASOL.DataService.Domain.Model.DataModelFieldType.CurrencyNumber,
      Contracts.DataModelFieldType.SingleSelectOptionSet => ASOL.DataService.Domain.Model.DataModelFieldType.SingleSelectOptionSet,
      Contracts.DataModelFieldType.MultiSelectOptionSet => ASOL.DataService.Domain.Model.DataModelFieldType.MultiSelectOptionSet,
      _ => ASOL.DataService.Domain.Model.DataModelFieldType.Text
    };
  }
}
