using System;
using System.Linq;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.DataModelAggregate;

namespace AVAIntegrationModeler.UseCases.DataModels.Mapping;

/// <summary>
/// Statická třída pro mapování mezi doménovým objektem pole datového modelu a jeho DTO.
/// </summary>
public static class DataModelFieldMapper
{
  /// <summary>
  /// Mapuje doménový objekt pole (<see cref="DataModelField"/>) na jeho datový přenosový objekt (<see cref="DataModelFieldDTO"/>).
  /// </summary>
  /// <param name="field"><see cref="DataModelField"/></param>
  /// <returns><see cref="DataModelFieldDTO"/></returns>
  public static DataModelFieldDTO MapToDataModelFieldDTO(DataModelField field)
  {
    if (field == default) return default!;

    DataModelFieldDTO result = new DataModelFieldDTO()
    {
      Id = field.Id,
      Name = field.Name,
      Label = field.Label,
      Description = field.Description,
      IsPublishedForLookup = field.IsPublishedForLookup,
      IsCollection = field.IsCollection,
      IsLocalized = field.IsLocalized,
      IsNullable = field.IsNullable,
      FieldType = field.FieldType,
      ReferencedEntityTypeIds = field.ReferencedEntityTypeIds.ToList()
    };
    
    return result;
  }

  /// <summary>
  /// Mapuje DTO pole (<see cref="DataModelFieldDTO"/>) na doménový objekt (<see cref="DataModelField"/>).
  /// </summary>
  /// <param name="dto"><see cref="DataModelFieldDTO"/></param>
  /// <returns><see cref="DataModelField"/></returns>
  public static DataModelField? MapToEntity(DataModelFieldDTO? dto)
  {
    if (dto == default) return default;

    var field = new DataModelField(dto.Id, dto.Name, dto.FieldType);
    
    if (!string.IsNullOrEmpty(dto.Label))
      field.SetLabel(dto.Label);
    
    if (!string.IsNullOrEmpty(dto.Description))
      field.SetDescription(dto.Description);
    
    if (dto.IsPublishedForLookup)
      field.MarkAsPublishedForLookup();
    
    if (dto.IsCollection)
      field.MarkAsCollection();
    
    if (dto.IsLocalized)
      field.MarkAsLocalized();
    
    if (dto.IsNullable)
      field.MarkAsNullable();
    
    // Mapování referencí na entity
    foreach (var entityTypeId in dto.ReferencedEntityTypeIds)
    {
      field.AddReferencedEntityType(entityTypeId);
    }
    
    return field;
  }
}
