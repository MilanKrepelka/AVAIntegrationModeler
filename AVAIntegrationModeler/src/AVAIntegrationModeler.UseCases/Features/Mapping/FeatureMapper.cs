using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AVAIntegrationModeler.Contracts.DTO;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace AVAIntegrationModeler.UseCases.Features.Mapping;
public static class FeatureMapper
{
  /// <summary>
  /// Mapuje doménový objekt feature (<see cref="Domain.FeatureAggregate.Feature"/>) na jeho datový přenosový objekt (<see cref="FeatureSummaryDTO"/>).
  /// </summary>
  /// <param name="feature"><see cref="Domain.FeatureAggregate.Feature"/></param>
  /// <returns><see cref="FeatureSummaryDTO"/></returns>
  public static FeatureSummaryDTO MapToFeatureSummaryDTO(Domain.FeatureAggregate.Feature? feature)
  {
    Guard.Against.Null(feature, nameof(feature));

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
  public static FeatureDTO MapToFeatureDTO(Domain.FeatureAggregate.Feature feature, List<FeatureSummaryDTO> features, List<DataModelSummaryDTO> models)
  {
    Guard.Against.Null(feature, nameof(feature));

    var result = new FeatureDTO()
    {
      Id = feature.Id,
      Code = feature.Code,
      Name = LocalizedValueMapper.MapToDTO(feature.Name),
      Description = LocalizedValueMapper.MapToDTO(feature.Description),
      
      IncludedFeatures = feature.IncludedFeatures?.Select(inc => new IncludedFeatureDTO
      {
          Feature = features.FirstOrDefault(item=>item.Id == inc.FeatureId)?? FeatureSummaryDTO.Empty,
          ConsumeOnly = inc.ConsumeOnly
      }).ToList() ?? new List<IncludedFeatureDTO>(),
      
      IncludedModels = feature.IncludedModels?.Select(inc => new IncludedDataModelDTO
      {
          DataModel = models.FirstOrDefault(item=>item.Id == inc.ModelId)?? DataModelSummaryDTO.Empty,
          ReadOnly = inc.ReadOnly
      }).ToList() ?? new List<IncludedDataModelDTO>()
    };
    return result;
  }

  /// <summary>
  /// Mapuje objekt feature (<see cref="FeatureDTO"/>) na jeho datový přenosový objekt (<see cref="FeatureDTO"/>).
  /// </summary>
  /// <param name="feature"><see cref="Domain.FeatureAggregate.Feature"/></param>
  /// <returns><see cref="FeatureSummaryDTO"/></returns>
  public static FeatureSummaryDTO MapToFeatureSummaryDTO(FeatureDTO feature)
  {
    Guard.Against.Null(feature, nameof(feature));

    var result = new FeatureSummaryDTO()
    {
      Id = feature.Id,
      Code = feature.Code,
    };
    return result;
  }

}
