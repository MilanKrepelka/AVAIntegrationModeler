using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using AVAIntegrationModeler.Contracts.DTO;

namespace AVAIntegrationModeler.AVAPlace.Mapping;
public static class IncludedFeatureMapper
{
  /// <summary>
  /// Převede datový přenosový objekt zahrnuté feature (<see cref="ASOL.DataService.Contracts.IntegrationFeatureModel.IncludedFeatureModel"/>) na jeho datový přenosový objekt (<see cref="Contracts.DTO.IncludedFeatureDTO"/>).
  /// </summary>
  /// <param name="includedFeatureModel"><see cref="ASOL.DataService.Contracts.IntegrationFeatureModel.IncludedFeatureModel"/></param>
  /// <param name="features">Features které se používají k doplňování</param>
  /// <returns><see cref="Contracts.DTO.IncludedFeatureDTO"/></returns>
  public static Contracts.DTO.IncludedFeatureDTO IncludedFeatureDTO(ASOL.DataService.Contracts.IntegrationFeatureDefinition.IncludedFeature includedFeatureModel , IEnumerable<Contracts.DTO.FeatureSummaryDTO> features)
  {
    Guard.Against.Null(includedFeatureModel, $"{nameof(IncludedFeatureMapper)} - {nameof(includedFeatureModel)}");
    
    FeatureSummaryDTO featureSummary = Contracts.DTO.FeatureSummaryDTO.Empty;
    if (Guid.TryParse(includedFeatureModel.CodeOrId, out Guid guid))
    {
      featureSummary = features.FirstOrDefault(f => f.Id == Guid.Parse(includedFeatureModel.CodeOrId)) ?? FeatureSummaryDTO.Empty;
    }
    else
    {
      featureSummary = features.FirstOrDefault(f => f.Code == includedFeatureModel.CodeOrId) ?? FeatureSummaryDTO.Empty;
    }

    return new Contracts.DTO.IncludedFeatureDTO()
    {
      ConsumeOnly = includedFeatureModel.ConsumeOnly,
      Feature = featureSummary ?? Contracts.DTO.FeatureSummaryDTO.Empty,
    };
  }
}
