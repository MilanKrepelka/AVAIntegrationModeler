using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace AVAIntegrationModeler.UseCases.Scenarios.Mapping;
public static class FeatureMapper
{
  /// <summary>
  /// Mapuje doménový objekt feature (<see cref="Domain.FeatureAggregate.Feature"/>) na jeho datový přenosový objekt (<see cref="FeatureSummaryDTO"/>).
  /// </summary>
  /// <param name="feature"><see cref="Domain.FeatureAggregate.Feature"/></param>
  /// <returns><see cref="FeatureSummaryDTO"/></returns>
  public static FeatureSummaryDTO? MapToFeatureSummaryDTO(Domain.FeatureAggregate.Feature? feature)
  {
    if (feature == default) return default;

    FeatureSummaryDTO result = new FeatureSummaryDTO()
    {
      Id = feature.Id,
      Code = feature.Code,
    };
    return result;
  }

  /// <summary>
  /// Mapuje doménový objekt feature (<see cref="Domain.FeatureAggregate.Feature"/>) na jeho datový přenosový objekt (<see cref="FeatureDTO"/>).
  /// </summary>
  /// <param name="feature"><see cref="Domain.FeatureAggregate.Feature"/></param>
  /// <returns><see cref="FeatureSummaryDTO"/></returns>
  public static FeatureDTO? MapToFeatureDTO(Domain.FeatureAggregate.Feature? feature)
  {
    if (feature == default) return default;

    FeatureDTO result = new FeatureDTO()
    {
      Id = feature.Id,
      Code = feature.Code,
      Name = LocalizedValueMapper.MapToDTO(feature.Name),
      Description = LocalizedValueMapper.MapToDTO(feature.Description),
      
    };
    return result;
  }

}
