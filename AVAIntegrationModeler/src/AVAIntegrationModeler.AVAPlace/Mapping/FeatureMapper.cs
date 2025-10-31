using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using ASOL.DataService.Connector;
using ASOL.DataService.Contracts;
using AVAIntegrationModeler.Contracts;
using AVAIntegrationModeler.Contracts.DTO;


namespace AVAIntegrationModeler.AVAPlace.Mapping;
/// <summary>
/// Statická třída pro mapování mezi doménovým objektem scénáře (<see cref="AVAIntegrationModeler.Contracts"/>) a jeho datovým přenosovým objektem (<see cref="FeatureDTO"/>).
/// Poskytuje metody pro převod mezi těmito dvěma reprezentacemi.
/// </summary>
public static class FeatureMapper
{
  /// <summary>
  /// Převede datový přenosový objekt feature (<see cref="IntegrationFeatureSummary"/>) na jeho datový přenosový objekt (<see cref="FeatureSummaryDTO"/>).
  /// </summary>
  /// <param name="integrationDefenitionSummary"><see cref="IntegrationFeatureSummary"/></param>
  /// <returns><see cref="FeatureSummaryDTO"/></returns>
  public static FeatureSummaryDTO FeatureSummaryDTO(IntegrationFeatureSummary integrationDefenitionSummary)
  {
    Guard.Against.Null(integrationDefenitionSummary, $"{nameof(FeatureMapper)} - {nameof(integrationDefenitionSummary)}");
    Guard.Against.NullOrEmpty(integrationDefenitionSummary.Id, $"{nameof(FeatureMapper)} - {nameof(integrationDefenitionSummary)} - {nameof(integrationDefenitionSummary.Id)}");

    return new FeatureSummaryDTO()
    {
      Code = integrationDefenitionSummary.Code,
      Id = Guid.Parse(integrationDefenitionSummary.Id),
    };
  }

  /// <summary>
  /// Převede datový přenosový objekt feature (<see cref="IntegrationFeatureSummary"/>) na jeho datový přenosový objekt (<see cref="FeatureDTO"/>).
  /// </summary>
  /// <param name="integrationDefenitionSummary"><see cref="IntegrationFeatureSummary"/></param>
  /// <returns><see cref="FeatureDTO"/></returns>
  public static FeatureDTO FeatureDTO(IntegrationFeatureSummary integrationDefenitionSummary)
  {
    Guard.Against.Null(integrationDefenitionSummary, $"{nameof(FeatureMapper)} - {nameof(integrationDefenitionSummary)}");
    Guard.Against.NullOrEmpty(integrationDefenitionSummary.Id, $"{nameof(FeatureMapper)} - {nameof(integrationDefenitionSummary)} - {nameof(integrationDefenitionSummary.Id)}");

    return new FeatureDTO()
    {
      Code = integrationDefenitionSummary.Code,
      Id = Guid.Parse(integrationDefenitionSummary.Id),
    };
  }

  /// <summary>
  /// Převede datový přenosový objekt feature (<see cref="IntegrationFeatureDefinition"/>) na jeho datový přenosový objekt (<see cref="FeatureDTO"/>).
  /// </summary>
  /// <param name="integrationFeatureDefenition"><see cref="IntegrationFeatureDefinition"/></param>
  /// <param name="features">Features které se používají k doplňování</param>
  /// <returns><see cref="FeatureDTO"/></returns>
  public static FeatureDTO FeatureDTO(IntegrationFeatureDefinition integrationFeatureDefenition, IEnumerable<FeatureSummaryDTO> features, IEnumerable<DataModelSummaryDTO> dataModels)
  {
    Guard.Against.Null(integrationFeatureDefenition, $"{nameof(FeatureMapper)} - {nameof(integrationFeatureDefenition)}");
    Guard.Against.NullOrEmpty(integrationFeatureDefenition.Id, $"{nameof(FeatureMapper)} - {nameof(integrationFeatureDefenition)} - {nameof(integrationFeatureDefenition.Id)}");

    return new FeatureDTO()
    {
      Code = integrationFeatureDefenition.Code,
      Id = Guid.Parse(integrationFeatureDefenition.Id),
      Name = LocalizedValueMapper.MapToDTO(integrationFeatureDefenition.Name),
      Description = LocalizedValueMapper.MapToDTO(integrationFeatureDefenition.Description),

      IncludedFeatures = integrationFeatureDefenition.IncludedFeatures?.Select(inc => IncludedFeatureMapper.IncludedFeatureDTO(inc, features)
      ).ToList() ?? new List<IncludedFeatureDTO>(),

      IncludedModels = integrationFeatureDefenition.IncludedModels?.Select(inc => IncludedModelMapper.IncludedDataModelDTO(inc, dataModels)
      ).ToList() ?? new List<IncludedDataModelDTO>()
    };
  }
}
