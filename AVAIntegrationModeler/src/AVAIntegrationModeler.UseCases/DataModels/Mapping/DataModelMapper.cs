using System;
using System.Linq;
using Ardalis.GuardClauses;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.DataModelAggregate;

namespace AVAIntegrationModeler.UseCases.DataModels.Mapping;

/// <summary>
/// Statická třída pro mapování mezi doménovým objektem datového modelu a jeho DTO.
/// </summary>
public static class DataModelMapper
{
  /// <summary>
  /// Mapuje doménový objekt datového modelu (<see cref="DataModel"/>) na jeho datový přenosový objekt (<see cref="DataModelDTO"/>).
  /// </summary>
  /// <param name="dataModel"><see cref="DataModel"/></param>
  /// <returns><see cref="DataModelDTO"/></returns>
  public static DataModelDTO MapToDataModelDTO(DataModel dataModel)
  {
    Guard.Against.Null(dataModel, nameof(dataModel));

    DataModelDTO result = new DataModelDTO()
    {
      Id = dataModel.Id,
      Code = dataModel.Code,
      Name = dataModel.Name,
      Description = dataModel.Description,
      Notes = dataModel.Notes,
      IsAggregateRoot = dataModel.IsAggregateRoot,
      AreaId = dataModel.AreaId,
      Fields = dataModel.Fields.Select(DataModelFieldMapper.MapToDataModelFieldDTO).ToList()
    };
    
    return result;
  }

  /// <summary>
  /// Mapuje DTO datového modelu (<see cref="DataModelDTO"/>) na doménový objekt (<see cref="DataModel"/>).
  /// </summary>
  /// <param name="dto"><see cref="DataModelDTO"/></param>
  /// <returns><see cref="DataModel"/></returns>
  public static DataModel MapToEntity(DataModelDTO dto)
  {
    Guard.Against.Null(dto, nameof(dto));

    var dataModel = new DataModel(dto.Id, dto.Code);
    
    if (!string.IsNullOrEmpty(dto.Name))
      dataModel.SetName(dto.Name);
    
    if (!string.IsNullOrEmpty(dto.Description))
      dataModel.SetDescription(dto.Description);
    
    if (!string.IsNullOrEmpty(dto.Notes))
      dataModel.SetNotes(dto.Notes);
    
    if (dto.IsAggregateRoot)
      dataModel.MarkAsAggregateRoot();
    
    if (dto.AreaId.HasValue)
      dataModel.SetArea(dto.AreaId);
    
    // Mapování fields
    foreach (var fieldDto in dto.Fields)
    {
      var field = DataModelFieldMapper.MapToEntity(fieldDto);
      if (field != null)
        dataModel.AddField(field);
    }
    
    return dataModel;
  }

  /// <summary>
  /// Mapuje doménový objekt datového modelu (<see cref="DataModel"/>) na jeho sumarizační DTO (<see cref="DataModelSummaryDTO"/>).
  /// </summary>
  /// <param name="dataModel"><see cref="DataModel"/></param>
  /// <returns><see cref="DataModelSummaryDTO"/></returns>
  public static DataModelSummaryDTO MapToDataModelSummaryDTO(DataModel dataModel)
  {
    Guard.Against.Null(dataModel, nameof(dataModel));

    DataModelSummaryDTO result = new DataModelSummaryDTO()
    {
      Id = dataModel.Id,
      Code = dataModel.Code,
      Name = dataModel.Name,
    };
    
    return result;
  }
}
