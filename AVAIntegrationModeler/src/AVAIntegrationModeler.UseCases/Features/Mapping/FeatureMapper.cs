using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AVAIntegrationModeler.Contracts.DTO;
using AVAIntegrationModeler.UseCases.Scenarios.Mapping;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace AVAIntegrationModeler.UseCases.Features.Mapping;
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

    var result = new FeatureSummaryDTO()
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
  public static FeatureDTO MapToFeatureDTO(Domain.FeatureAggregate.Feature feature)
  {
    Guard.Against.Null(feature, nameof(feature));

    var result = new FeatureDTO()
    {
      Id = feature.Id,
      Code = feature.Code,
      Name = LocalizedValueMapper.MapToDTO(feature.Name),
      Description = LocalizedValueMapper.MapToDTO(feature.Description),
    };
    return result;
  }

}
