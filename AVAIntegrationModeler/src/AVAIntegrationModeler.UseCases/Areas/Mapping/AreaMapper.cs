using System;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.Domain.AreaAggregate;

namespace AVAIntegrationModeler.UseCases.Areas.Mapping;

/// <summary>
/// Statická třída pro mapování mezi doménovým objektem oblasti a jeho DTO.
/// </summary>
public static class AreaMapper
{
  /// <summary>
  /// Mapuje doménový objekt oblasti (<see cref="Area"/>) na jeho datový přenosový objekt (<see cref="AreaDTO"/>).
  /// </summary>
  /// <param name="area"><see cref="Area"/></param>
  /// <returns><see cref="AreaDTO"/></returns>
  public static AreaDTO? MapToAreaDTO(Area? area)
  {
    if (area == default) return default;

    AreaDTO result = new AreaDTO()
    {
      Id = area.Id,
      Code = area.Code,
      Name = area.Name
    };
    
    return result;
  }

  /// <summary>
  /// Mapuje DTO oblasti (<see cref="AreaDTO"/>) na doménový objekt (<see cref="Area"/>).
  /// </summary>
  /// <param name="dto"><see cref="AreaDTO"/></param>
  /// <returns><see cref="Area"/></returns>
  public static Area? MapToEntity(AreaDTO? dto)
  {
    if (dto == default) return default;

    var area = new Area(dto.Id, dto.Code);
    
    if (!string.IsNullOrEmpty(dto.Name))
    {
      area.SetName(dto.Name);
    }
    
    return area;
  }
}
